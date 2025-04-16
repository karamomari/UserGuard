global using System;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using MediatR;
global using UserGuard_API.Events;
global using AutoMapper;

global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;

global using UserGuard_API.Model;
global using UserGuard_API.Controllers;
global using UserGuard_API.DTO;
global using UserGuard_API.Model;
global using UserGuard_API.Repositories.Interfaces;
global using UserGuard_API.Service;