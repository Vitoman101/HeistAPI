using HeistAPI.Entities.Dto;
using HeistAPI.Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace HeistAPI.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IActionResult> GetAllMembers();
        Task<IActionResult> GetMemberById(int memberId);
        Task<IActionResult> GetMemberSkills(int memberId);
        Task<IActionResult> PostMember(Member member);
        Task<IActionResult> UpdateMemberSkills(int memberId, MemberSkillsDto skillsUpdateDto);
        Task<IActionResult> DeleteMemberSkill(int memberId, string skillName);
    }

}
