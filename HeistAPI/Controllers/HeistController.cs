using HeistAPI.Data;
using HeistAPI.Entities.Dto;
using HeistAPI.Entities.Enum;
using HeistAPI.Entities.Models;
using HeistAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeistAPI.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class HeistController : ControllerBase
    {
        private readonly IHeistService _heistService;

        public HeistController(IHeistService heistService)
        {
            _heistService = heistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHeists()
        {
            return await _heistService.GetAllHeistsAsync();
        }

        [HttpGet("{heistId}")]
        public async Task<IActionResult> GetHeistById(int heistId)
        {
            return await _heistService.GetHeistByIdAsync(heistId);
        }



        [HttpGet("{heistId}/members")]
        public async Task<IActionResult> GetHeistMembers(int heistId)
        {
            return await _heistService.GetHeistMembersAsync(heistId);
        }

        [HttpPost]
        public async Task<IActionResult> PostHeist(Heist heist)
        {
            return await _heistService.PostHeistAsync(heist);
        }



        [HttpGet("{heistId}/skills")]
        public async Task<IActionResult> GetHeistSkills(int heistId)
        {
            return await _heistService.GetHeistSkillsAsync(heistId);
        }


        [HttpGet("{heistId}/status")]
        public async Task<IActionResult> GetHeistStatus(int heistId)
        {
            return await _heistService.GetHeistStatusAsync(heistId);
        }

        [HttpPatch("{heistId}/skills")]
        public async Task<IActionResult> UpdateHeistSkills(int heistId, HeistSkillsUpdateDto skillsUpdateDto)
        {
            return await _heistService.UpdateHeistSkillsAsync(heistId, skillsUpdateDto);
        }
        [HttpPut("{heistId}/start")]
        public async Task<IActionResult> StartHeist(int heistId)
        {
            return await _heistService.StartHeistAsync(heistId);
        }
        [HttpGet("{heistId}/eligible_members")]
        public async Task<IActionResult> GetEligibleMembers(int heistId)
        {
            return await _heistService.GetEligibleMembersAsync(heistId);
        }

        [HttpPut("{heistId}/members")]
        public async Task<IActionResult> ConfirmMembersForHeist(int heistId, ConfirmMembersDto dto)
        {
            return await _heistService.ConfirmMembersForHeistAsync(heistId, dto);
        }

        [HttpPut("{heistId}/outcome")]
        public async Task<IActionResult> GetHeistOutcome(int heistId)
        {
            return await _heistService.GetHeistOutcomeAsync(heistId);
        }

    }
}
