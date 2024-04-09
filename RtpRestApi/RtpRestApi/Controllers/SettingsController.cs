using RtpRestApi.Models;
using RtpRestApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace RtpRestApi.Controllers;

[ApiController]
[Route("api/setting")]
public class SettingsController : ControllerBase
{
    private readonly SettingsService _settingsService;

    public SettingsController(SettingsService SettingsService) =>
        _settingsService = SettingsService;

    [HttpGet]
    [Route("listAll")]
    public async Task<IActionResult> Get()
    {
        var result = await _settingsService.GetAsync();
        if (result == null)
        {
            return NoContent();
        }

        return Ok(new
        {
            success = true,
            result = result,
            message = "Successfully found all documents",
        });
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Setting>> Get(string id)
    {
        var Setting = await _settingsService.GetAsync(id);

        if (Setting is null)
        {
            return NotFound();
        }

        return Setting;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Setting newSetting)
    {
        await _settingsService.CreateAsync(newSetting);

        return CreatedAtAction(nameof(Get), new { id = newSetting._id }, newSetting);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Setting updatedSetting)
    {
        var Setting = await _settingsService.GetAsync(id);

        if (Setting is null)
        {
            return NotFound();
        }

        updatedSetting._id = Setting._id;

        await _settingsService.UpdateAsync(id, updatedSetting);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var Setting = await _settingsService.GetAsync(id);

        if (Setting is null)
        {
            return NotFound();
        }

        await _settingsService.RemoveAsync(id);

        return NoContent();
    }
}