using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Shared.Constants;
using Shared.Interfaces;

namespace Shared.Services;

/// <summary>
/// Предоставляет информацию о текущем пользователе на основе HTTP-заголовков.
/// </summary>
public class UserContext(IHttpContextAccessor httpContextAccessor, IHostEnvironment hostEnvironment) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

    /// <summary>
    /// Получает идентификатор пользователя из заголовка запроса.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">Если заголовок отсутствует.</exception>
    /// <exception cref="FormatException">Если значение заголовка не является корректным Guid.</exception>
    /// <exception cref="InvalidOperationException">Если HttpContext недоступен.</exception>
    public Guid UserId
    {
        get
        {
            if (_hostEnvironment.IsDevelopment())
            {
                return Guid.Parse("00000000-0000-0000-0000-000000000001");
            }

            var context = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HttpContext is not available");

            if (!context.Request.Headers.TryGetValue(CustomHeaders.UserId, out var userIdValue))
                throw new UnauthorizedAccessException($"Заголовок {CustomHeaders.UserId} обязателен.");

            return !Guid.TryParse(userIdValue, out var userId)
                ? throw new FormatException($"Некорректный формат {CustomHeaders.UserId}.")
                : userId;
        }
    }
}