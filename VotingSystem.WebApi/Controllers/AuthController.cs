using System.ComponentModel.DataAnnotations;
using AutoMapper;
using VotingSystem.DataAccess.Models;
using VotingSystem.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.DataAccess.Services;
using VotingSystem.DataAccess;
using VotingSystem.WebApi.Infrastructure;

namespace VotingSystem.WebApi.Controllers;

/// <summary>
/// UsersController
/// </summary>
[ApiController]
[Route("/users")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    private readonly IUserAccountService _userAccountService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="usersService"></param>
    public UsersController(IMapper mapper, IUsersService usersService, IUserAccountService userAccountService)
    {
        _mapper = mapper;
        _usersService = usersService;
        _userAccountService = userAccountService;
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="userRequestDto"></param>
    /// <response code="201">User created successfully</response>
    /// <response code="400">Bad Request</response>
    /// <response code="409">Conflict</response>
    [HttpPost]
    [ProducesResponseType(statusCode: StatusCodes.Status201Created, type: typeof(UserResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] UserRequestDto userRequestDto)
    {
        var user = _mapper.Map<User>(userRequestDto);

        await _usersService.AddUserAsync(user, userRequestDto.Password);

        var userResponseDto = _mapper.Map<UserResponseDto>(user);

        return StatusCode(StatusCodes.Status201Created, userResponseDto);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">User</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden</response>
    /// <response code="404">Not found</response>
    [HttpGet]
    [Route("{id}")]
    [Authorize]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(UserResponseDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser([FromRoute][Required] string id)
    {
        var user = await _usersService.GetUserByIdAsync(id);
        var userResponseDto = _mapper.Map<UserResponseDto>(user);

        return Ok(userResponseDto);
    }

    [HttpPost]
    [Route("login")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(LoginResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        try
        {
            var (authToken, refreshToken, userId) = await _usersService.LoginAsync(loginRequestDto.Email, loginRequestDto.Password);

            var loginResponseDto = new LoginResponseDto
            {
                UserId = userId,
                AuthToken = authToken,
                RefreshToken = refreshToken,
            };

            return Ok(loginResponseDto);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            // Log the exception for debugging purposes
            Console.WriteLine($"Unexpected error during login: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (string.IsNullOrWhiteSpace(registerDto.Email))
        {
            return BadRequest("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(registerDto.Password))
        {
            return BadRequest("Password is required.");
        }

        try
        {
            var (authToken, refreshToken, userId) = await _usersService.RegisterAsync(registerDto.Email, registerDto.Password);

            var loginResponseDto = new LoginResponseDto
            {
                AuthToken = authToken,
                RefreshToken = refreshToken,
                UserId = userId
            };

            return Ok(loginResponseDto);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return Conflict(ex.Message); // 409 Conflict for existing user  
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message); // 400 Bad Request for other validation errors  
        }
        catch (Exception ex)
        {
            // Log the exception for debugging purposes  
            Console.WriteLine($"Unexpected error during registration: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }


    /// <summary>
    /// Logout
    /// </summary>
    /// <response code="200">Logout was successful</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost]
    [Route("logout")]
    [Authorize]
    [ProducesResponseType(statusCode: StatusCodes.Status204NoContent, type: typeof(void))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        await _usersService.LogoutAsync();

        return NoContent();
    }

    /// <summary>
    /// Redeem refresh token
    /// </summary>
    /// <response code="200">Login was successful</response>
    [HttpPost]
    [Route("refresh")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(UserResponseDto))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RedeemRefreshToken([FromBody] string refreshToken)
    {
        var (authToken, newRefreshToken, userId) = await _usersService.RedeemRefreshTokenAsync(refreshToken);

        var loginResponseDto = new LoginResponseDto
        {
            UserId = userId,
            AuthToken = authToken,
            RefreshToken = newRefreshToken,
        };

        return Ok(loginResponseDto);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
    {
        var success = await _userAccountService.SendResetPasswordLinkAsync(dto.Email);
        return Ok(new { success });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
    {
        var success = await _userAccountService.ResetPasswordAsync(dto.Email, dto.Token, dto.NewPassword);
        if (!success)
            return BadRequest("Invalid token or user not found");

        return Ok("Password has been reset");
    }

}
