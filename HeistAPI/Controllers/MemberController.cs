using HeistAPI.Entities.Dto;
using HeistAPI.Entities.Models;
using HeistAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HeistAPI.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMembers()
        {
            return await _memberService.GetAllMembers();
        }

        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetMemberById(int memberId)
        {
            return await _memberService.GetMemberById(memberId);
        }

        [HttpGet("{memberId}/skills")]
        public async Task<IActionResult> GetMemberSkills(int memberId)
        {
            return await _memberService.GetMemberSkills(memberId);
        }

        [HttpPost]
        public async Task<IActionResult> PostMember(Member member)
        {
            return await _memberService.PostMember(member);
        }

        [HttpPut("{memberId}/skills")]
        public async Task<IActionResult> UpdateMemberSkills(int memberId, MemberSkillsDto skillsUpdateDto)
        {
            return await _memberService.UpdateMemberSkills(memberId, skillsUpdateDto);
        }

        [HttpDelete("{memberId}/skills/{skillName}")]
        public async Task<IActionResult> DeleteMemberSkill(int memberId, string skillName)
        {
            return await _memberService.DeleteMemberSkill(memberId, skillName);
        }
    }
}
