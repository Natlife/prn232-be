using System.Collections.Generic;
using BusinessObjects.Models;
using Repositories;

namespace Services;

public class DepositCaptchaService : IDepositCaptchaService
{
    private readonly IDepositCaptchaRepository _repo;

    public DepositCaptchaService(IDepositCaptchaRepository repo)
    {
        _repo = repo;
    }

    public IEnumerable<DepositCaptcha> GetAllCaptchas() => _repo.GetAllCaptchas();

    public DepositCaptcha? GetCaptchaById(int id) => _repo.GetCaptchaById(id);

    public void AddCaptcha(DepositCaptcha captcha) => _repo.AddCaptcha(captcha);

    public DepositCaptcha GenerateCaptcha(int carId, string? code) => _repo.GenerateCaptcha(carId, code);
}
