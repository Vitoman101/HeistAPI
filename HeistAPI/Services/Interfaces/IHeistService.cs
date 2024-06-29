using HeistAPI.Entities.Dto;
using HeistAPI.Entities.Models;
using Microsoft.AspNetCore.Mvc;


namespace HeistAPI.Services.Interfaces
{
    public interface IHeistService
    {
        Task<IActionResult> GetAllHeistsAsync();
        Task<IActionResult> GetHeistByIdAsync(int heistId);
        Task<IActionResult> GetHeistMembersAsync(int heistId);
        Task<IActionResult> GetHeistSkillsAsync(int heistId);
        Task<IActionResult> GetHeistStatusAsync(int heistId);
        Task<IActionResult> PostHeistAsync(Heist heist);
        Task<IActionResult> UpdateHeistSkillsAsync(int heistId, HeistSkillsUpdateDto skillsUpdateDto);
        Task<IActionResult> GetEligibleMembersAsync(int heistId);
        Task<IActionResult> ConfirmMembersForHeistAsync(int heistId, ConfirmMembersDto dto);
        Task<IActionResult> StartHeistAsync(int heistId);
        Task<IActionResult> GetHeistOutcomeAsync(int heistId);
    }
}
