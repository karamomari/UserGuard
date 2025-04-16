
namespace UserGuard_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly SymmetricSecurityKey key;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;



        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            SymmetricSecurityKey key,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.key = key;
            this.configuration = configuration;
            _roleManager = roleManager;
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = "Email is already registered."
                });
            }

            var existingUsername = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUsername != null)
            {
                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = "Username is already taken."
                });
            }


            var existingPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == registerDto.Phone);
            if (existingPhone != null)
            {
                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = "Phone number is already used."
                });
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.Phone,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                EmailConfirmed = true,
                IsActive = true,
                AccountCreatedAt = DateTime.UtcNow,
                LockoutEnabled = false,
              
                //
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = result.Errors.Select(e => e.Description)
                });
            }

            #region
            //var roleExists = await _roleManager.RoleExistsAsync("Admin");
            //if (!roleExists)
            //{
            //    var role = new IdentityRole("Admin");
            //    await _roleManager.CreateAsync(role);
            //}
            //await _userManager.AddToRoleAsync(user, "Admin");
            #endregion

            return Ok(new GeneralResponse
            {
                Success = true,
                Data = "Registration successful. Awaiting admin activation."
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto userFromReq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage)
                });
            }

            var userFromDB = await _userManager.FindByNameAsync(userFromReq.UserName);

            if (userFromDB == null)
            {
                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = "Invalid username or password"
                });
            }

            if (!userFromDB.IsActive)
            {
                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = "Your account is not activated yet."
                });
            }

            if (await _userManager.IsLockedOutAsync(userFromDB))
            {
                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = "Account is locked. Try again later."
                });
            }

            if (await _userManager.CheckPasswordAsync(userFromDB, userFromReq.Password))
            {
                await _userManager.ResetAccessFailedCountAsync(userFromDB);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userFromDB.Id),
                    new Claim(ClaimTypes.Name, userFromDB.UserName)
                };

                var roles = await _userManager.GetRolesAsync(userFromDB);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var token = new JwtSecurityToken(
                    issuer: configuration["JWT:IssuerIP"],
                    audience: configuration["JWT:AudienceIP"],
                    expires: DateTime.UtcNow.AddHours(1),
                    claims: claims,
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new ToReturnToken
                {
                    Expired = token.ValidTo,
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            else
            {
                await _userManager.AccessFailedAsync(userFromDB);

                int accessFailedCount = await _userManager.GetAccessFailedCountAsync(userFromDB);
                int maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;
                int remainingAttempts = maxAttempts - accessFailedCount;

                if (await _userManager.IsLockedOutAsync(userFromDB))
                {
                    return BadRequest(new GeneralResponse
                    {
                        Success = false,
                        Data = "Account locked due to multiple failed attempts."
                    });
                }

                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = $"Invalid username or password. You have {remainingAttempts} attempt(s) left before your account gets locked."
                });
            }
        }



        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("No user found with this email.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"http://localhost:5017/reset-password?email={dto.Email}&token={Uri.EscapeDataString(token)}";


            await _mediator.Publish(new UserRegisteredEvent(user.Email, "Reset Password", resetLink));

            return Ok(new GeneralResponse { Success=true, Data= "Please check your email." });
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Invalid email.");
            
            //string decodedToken = Uri.UnescapeDataString(dto.Token);
            //or external api to decode token
            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password has been reset successfully.");
        }



    }
}
