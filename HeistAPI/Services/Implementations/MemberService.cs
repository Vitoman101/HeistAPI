using HeistAPI.Data;
using HeistAPI.Entities.Dto;
using HeistAPI.Entities.Models;
using HeistAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeistAPI.Services.Implementations
{
    public class MemberService : IMemberService
    {
        private readonly DataContext _context;

        public MemberService(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetAllMembers()
        {
            var members = await _context.Members.Include(p => p.Skills).ToListAsync();
            if(members == null || members.Count == 0)
            {
                return new NotFoundObjectResult("No members were found");
            }

            return new OkObjectResult(members);
        }

        public async Task<IActionResult> GetMemberById(int memberId)
        {
            var member = await _context.Members.Include(p => p.Skills).FirstOrDefaultAsync(m => m.Id == memberId);
            if (member == null)
            {
                return new NotFoundObjectResult($"Member with ID {memberId} not found.");
            }
            return new OkObjectResult(member);
        }

        public async Task<IActionResult> GetMemberSkills(int memberId)
        {
            var member = await _context.Members.Include(m => m.Skills).FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
            {
                return new NotFoundObjectResult($"Member with ID {memberId} not found.");
            }

            var skills = member.Skills.Select(s => new MemberSkillDto
            {
                Name = s.Name,
                Level = s.Level
            }).ToList();

            var memberSkillsDto = new MemberSkillsDto
            {
                Skills = skills,
                MainSkill = member.MainSkill
            };

            return new OkObjectResult(memberSkillsDto);
        }

        public async Task<IActionResult> PostMember(Member member)
        {
            var memberDto = new Member
            {
                Name = member.Name,
                Sex = member.Sex,
                Email = member.Email,
                MainSkill = member.MainSkill,
                Status = member.Status,
                Skills = new List<MemberSkill>()
            };

            if (member.Skills != null)
            {
                foreach (var skillDto in member.Skills)
                {
                    var skill = new MemberSkill
                    {
                        Name = skillDto.Name,
                        Level = skillDto.Level,
                        MemberId = member.Id
                    };
                    memberDto.Skills.Add(skill);
                }
            }

            if (!string.IsNullOrEmpty(memberDto.MainSkill))
            {
                var skillNames = memberDto.Skills.Select(s => s.Name.ToLower());
                if (!skillNames.Contains(memberDto.MainSkill.ToLower()))
                {
                    return new BadRequestObjectResult("MainSkill must match at least one of the skill names.");
                }
            }

            var emailAlreadyExists = await _context.Members.AnyAsync(s => s.Email == member.Email);

            if (emailAlreadyExists)
            {
                return new BadRequestObjectResult("Member with same email already exists");
            }

            _context.Members.Add(memberDto);
            await _context.SaveChangesAsync();

            var locationUri = new Uri($"/member/{memberDto.Id}", UriKind.Relative);

            return new CreatedResult(locationUri.ToString(), memberDto);
        }

        public async Task<IActionResult> UpdateMemberSkills(int memberId, MemberSkillsDto skillsUpdateDto)
        {
            var member = await _context.Members.Include(m => m.Skills).FirstOrDefaultAsync(m => m.Id == memberId);
            if (member == null)
            {
                return new NotFoundObjectResult($"Member with ID {memberId} not found.");
            }

            if (skillsUpdateDto.Skills == null)
            {
                return new BadRequestObjectResult("You must include some skills in request");
            }

            foreach (var skill in skillsUpdateDto.Skills)
            {
                var existingSkill = member.Skills.FirstOrDefault(s => s.Name.Equals(skill.Name, StringComparison.OrdinalIgnoreCase));
                if (existingSkill != null)
                {
                    if (existingSkill.Level != skill.Level)
                    {
                        existingSkill.Level = skill.Level;
                    }
                }
                else
                {
                    var newSkill = new MemberSkill
                    {
                        Name = skill.Name,
                        Level = skill.Level,
                        MemberId = memberId
                    };
                    member.Skills.Add(newSkill);
                }
            }

            if (!string.IsNullOrEmpty(skillsUpdateDto.MainSkill))
            {
                var skillNames = member.Skills.Select(s => s.Name.ToLower());
                if (!skillNames.Contains(skillsUpdateDto.MainSkill.ToLower()))
                {
                    return new BadRequestObjectResult("MainSkill must match at least one of the skill names.");
                }

                member.MainSkill = skillsUpdateDto.MainSkill;
            }

            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        public async Task<IActionResult> DeleteMemberSkill(int memberId, string skillName)
        {
            var member = await _context.Members.Include(m => m.Skills).FirstOrDefaultAsync(m => m.Id == memberId);
            if (member == null)
            {
                return new NotFoundObjectResult($"Member with ID {memberId} not found.");
            }

            var skillToRemove = member.Skills.FirstOrDefault(s => s.Name.Equals(skillName, StringComparison.OrdinalIgnoreCase));
            if (skillToRemove == null)
            {
                return new NotFoundObjectResult($"Skill '{skillName}' not found for member with ID {memberId}.");
            }

            if (member.MainSkill != null && member.MainSkill.Equals(skillName, System.StringComparison.OrdinalIgnoreCase))
            {
                member.MainSkill = null;
            }

            _context.MemberSkills.Remove(skillToRemove);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}
