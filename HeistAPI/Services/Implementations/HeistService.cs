using Azure;
using HeistAPI.Data;
using HeistAPI.Entities.Dto;
using HeistAPI.Entities.Enum;
using HeistAPI.Entities.Models;
using HeistAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeistAPI.Services.Implementations
{
    public class HeistService : IHeistService
    {
        private readonly DataContext _context;

        public HeistService(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ConfirmMembersForHeistAsync(int heistId, ConfirmMembersDto dto)
        {
            var heist = await _context.Heists
                .Include(h => h.Skills)
                .FirstOrDefaultAsync(h => h.Id == heistId);

            if (heist == null)
            {
                return new NotFoundObjectResult($"Heist with ID {heistId} not found.");
            }

            var membersToConfirm = new List<Member>();

            if (dto.Members == null)
            {
                return new BadRequestObjectResult("No members were provided in request");
            }

            foreach (var memberName in dto.Members)
            {
                var member = await _context.Members
                    .Include(m => m.Skills)
                    .FirstOrDefaultAsync(m => m.Name == memberName);

                if (member == null)
                {
                    return new BadRequestObjectResult($"Member '{memberName}' does not exist.");
                }

                if (member.Status != Status.AVAILABLE && member.Status != Status.RETIRED)
                {
                    return new BadRequestObjectResult($"Member '{memberName}' is not available or retired.");
                }

                var isMemberConfirmedForOtherHeist = await _context.Heists
                    .AnyAsync(h => h.Id != heistId && h.Status == Status.READY &&
                                   h.Members!.Any(m => m.Id == member.Id) &&
                                   !(heist.EndTime <= h.StartTime || heist.StartTime >= h.EndTime));

                if (isMemberConfirmedForOtherHeist)
                {
                    return new BadRequestObjectResult($"Member '{memberName}' is already confirmed for another heist happening at the same time.");
                }

                var isSkillMatch = heist.Skills.Any(hs => member.Skills.Any(ms => ms.Name == hs.Name && ms.Level!.Length >= hs.Level!.Length));

                if (!isSkillMatch)
                {
                    return new BadRequestObjectResult($"Member '{memberName}' does not have matching skills for this heist.");
                }

                membersToConfirm.Add(member);
            }

            heist.Status = Status.READY;
            heist.Members = membersToConfirm;

            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        public async Task<IActionResult> GetAllHeistsAsync()
        {
            var heists = await _context.Heists.Include(p => p.Skills)
                .Include(p => p.Members)
                .ToListAsync();

            if (heists == null || heists.Count == 0)
            {
                return new NotFoundObjectResult("No heists were found");
            }

            return new OkObjectResult(heists);
        }

        public async Task<IActionResult> GetEligibleMembersAsync(int heistId)
        {
            var heist = await _context.Heists.Include(h => h.Skills).FirstOrDefaultAsync(h => h.Id == heistId);
            if (heist == null)
            {
                return new NotFoundObjectResult($"Heist with ID {heistId} not found.");
            }

            var eligibleMembers = await _context.Members
                .Include(m => m.Skills)
                .Where(m => m.Status == Status.AVAILABLE || m.Status == Status.RETIRED)
                .ToListAsync();

            var skillsAggregate = heist.Skills
                .Select(g => new
                {
                    name = g.Name,
                    level = g.Level,
                    members = g.Members
                })
                .ToList();

            var response = new
            {
                skills = skillsAggregate.Select(s => new
                {
                    name = s.name,
                    level = s.level,
                    members = s.members
                }),
                members = eligibleMembers
                    .Where(m => m.Skills.Any(ms => heist.Skills.Any(hs => hs.Name == ms.Name && ms.Level!.Length >= hs.Level!.Length)))
                    .Select(m => new
                    {
                        name = m.Name,
                        skills = m.Skills.Select(ms => new
                        {
                            name = ms.Name,
                            level = ms.Level
                        })
                    })
            };

            return new OkObjectResult(response);

        }

        public async Task<IActionResult> GetHeistByIdAsync(int heistId)
        {
            var heist = await _context.Heists.Include(p => p.Skills).Include(m => m.Members).FirstOrDefaultAsync(h => h.Id == heistId);

            if (heist == null)
            {
                return new NotFoundObjectResult($"Heist with ID {heistId} not found.");
            }

            return new OkObjectResult(heist);
        }

        public async Task<IActionResult> GetHeistMembersAsync(int heistId)
        {
            var heist = await _context.Heists
                .Include(h => h.Members)
                .FirstOrDefaultAsync(h => h.Id == heistId);

            if (heist == null)
            {
                return new NotFoundObjectResult($"Heist with ID {heistId} not found.");
            }

            if (heist!.Status == Status.PLANNING)
            {
                return new ObjectResult("Heist is in PLANNING status.")
                {
                    StatusCode = 405
                };
            }

            if (heist.Members == null || !heist.Members.Any())
            {
                return new BadRequestObjectResult("A heist has no active members.");
            }

            var memberIds = heist.Members.Select(m => m.Id).ToList();
            var membersWithSkills = await _context.Members
                .Where(m => memberIds.Contains(m.Id))
                .Include(m => m.Skills)
                .ToListAsync();

            var memberDtos = membersWithSkills.Select(m => new MemberDto
            {
                Name = m.Name,
                Skills = m.Skills.Select(s => new MemberSkillDto
                {
                    Name = s.Name,
                    Level = s.Level!
                }).ToList()
            }).ToList();

            return new OkObjectResult(memberDtos);
        }

        public async Task<IActionResult> GetHeistSkillsAsync(int heistId)
        {
            var heist = await _context.Heists
                                      .Include(h => h.Skills)
                                      .FirstOrDefaultAsync(h => h.Id == heistId);

            if (heist == null)
            {
                return new NotFoundObjectResult($"Heist with ID {heistId} not found.");
            }

            var skills = heist.Skills.Select(s => new
            {
                Name = s.Name,
                Level = s.Level,
                Members = s.Members
            }).ToList();

            if(skills == null || skills.Count == 0)
            {
                return new NotFoundObjectResult($"Heist with ID {heistId} has no skills.");
            }

            return new OkObjectResult(skills);
        }

        public async Task<IActionResult> GetHeistStatusAsync(int heistId)
        {
            var heist = await _context.Heists.FirstOrDefaultAsync(h => h.Id == heistId);

            if (heist == null)
            {
                return new NotFoundObjectResult($"Heist with ID {heistId} not found.");
            }

            var status = new
            {
                Status = heist.Status,
            };

            return new NotFoundObjectResult(status);
        }

        public async Task<IActionResult> PostHeistAsync(Heist heist)
        {
            if (await _context.Heists.AnyAsync(h => h.Name == heist.Name))
            {
                return new BadRequestObjectResult("A heist with the same name already exists.");
            }

            if (heist.StartTime >= heist.EndTime)
            {
                return new BadRequestObjectResult("The start time must be before the end time.");
            }

            if (heist.EndTime < DateTime.UtcNow)
            {
                return new BadRequestObjectResult("The end time cannot be in the past.");
            }

            var duplicateSkills = heist.Skills.GroupBy(s => new { s.Name, s.Level })
                                  .Where(g => g.Count() > 1)
                                  .SelectMany(g => g)
                                  .ToList();

            if (duplicateSkills.Any())
            {
                return new BadRequestObjectResult("Duplicate skills with the same name and level are not allowed.");
            }


            var heistDto = new Heist
            {
                Name = heist.Name,
                Location = heist.Location,
                StartTime = heist.StartTime,
                EndTime = heist.EndTime,
                Status = heist.Status ?? Status.PLANNING,
                Skills = new List<HeistSkill>()
            };

            if (heist.Skills != null)
            {
                foreach (var skillDto in heist.Skills)
                {
                    var skill = new HeistSkill
                    {
                        Name = skillDto.Name,
                        Level = skillDto.Level,
                        Members = skillDto.Members,
                    };

                    heistDto.Skills.Add(skillDto);
                }
            }

            _context.Heists.Add(heistDto);
            await _context.SaveChangesAsync();

            var locationUri = new Uri($"/heist/{heistDto.Id}", UriKind.Relative);

            return new CreatedResult(locationUri, heistDto);

        }

        public async Task<IActionResult> StartHeistAsync(int heistId)
        {
            var heist = await _context.Heists.FirstOrDefaultAsync(h => h.Id == heistId);

            if (heist == null)
            {
                return new NotFoundObjectResult($"Heist with ID {heistId} not found.");
            }

            if (heist.Status != Status.READY)
            {
                return new ObjectResult("Heist status is not READY.")
                {
                    StatusCode = 405
                };
            }

            heist.Status = Status.IN_PROGRESS;
            await _context.SaveChangesAsync();

            return new OkObjectResult(heist);
        }

        public async Task<IActionResult> UpdateHeistSkillsAsync(int heistId, HeistSkillsUpdateDto skillsUpdateDto)
        {
            var heist = await _context.Heists.Include(h => h.Skills).FirstOrDefaultAsync(h => h.Id == heistId);
            if (heist == null)
            {
                return new NotFoundObjectResult($"Heist with ID {heistId} not found.");
            }

            if (skillsUpdateDto.Skills == null)
            {
                return new BadRequestObjectResult("No skills were provided in request");
            }

            var duplicateSkills = skillsUpdateDto.Skills.GroupBy(s => new { s.Name, s.Level })
                                        .Where(g => g.Count() > 1)
                                        .SelectMany(g => g)
                                        .ToList();
            if (duplicateSkills.Any())
            {
                return new BadRequestObjectResult("Duplicate skills with the same name and level are not allowed.");
            }

            foreach (var skillDto in skillsUpdateDto.Skills)
            {
                var existingSkill = heist.Skills.FirstOrDefault(s => s.Name.Equals(skillDto.Name, StringComparison.OrdinalIgnoreCase) && s.Level!.Equals(skillDto.Level, StringComparison.OrdinalIgnoreCase));
                if (existingSkill != null)
                {
                    existingSkill.Level = skillDto.Level;
                    existingSkill.Members = skillDto.Members;
                }
                else
                {
                    var newSkill = new HeistSkill
                    {
                        Name = skillDto.Name,
                        Level = skillDto.Level,
                        Members = skillDto.Members,
                        HeistId = heistId
                    };
                    heist.Skills.Add(newSkill);
                }
            }

            await _context.SaveChangesAsync();
            return new OkObjectResult(new {});
        }

        public async Task<IActionResult> GetHeistOutcomeAsync(int heistId)
        {
            var heist = await _context.Heists
                .Include(h => h.Skills)
                .Include(h => h.Members)
                .FirstOrDefaultAsync(h => h.Id == heistId);

            if (heist == null)
            {
                return new NotFoundObjectResult($"Heist with ID {heistId} not found.");
            }

            if (heist.Skills == null || !heist.Skills.Any())
            {
                return new BadRequestObjectResult("No skills have been defined for this heist.");
            }

            if (heist.Members == null || !heist.Members.Any())
            {
                return new BadRequestObjectResult("No members found in heist");
            }

            int totalMembersRequired = 0;

            foreach (var skill in heist.Skills)
            {
                totalMembersRequired += skill.Members;
            }

            int totalMembersConfirmed = heist.Members.Count();

            double percentageConfirmed = (double)totalMembersConfirmed / totalMembersRequired * 100;

            string outcome;

            if (percentageConfirmed < 50)
            {
                outcome = "FAILED";
            }
            else if (percentageConfirmed < 75)
            {
                outcome = "FAILED";
            }
            else if (percentageConfirmed < 100)
            {
                outcome = "SUCCEEDED";
            }
            else
            {
                outcome = "SUCCEEDED";
            }

            Random random = new Random();
            foreach (var member in heist.Members)
            {
                if (random.Next(2) == 0) 
                {
                    member.Status = Status.EXPIRED;
                }
                else
                {
                    member.Status = Status.INCARCERATED;
                }
            }

            await _context.SaveChangesAsync();

            var heistOutcomeDto = new
            {
                Outcome = outcome
            };

            return new OkObjectResult(heistOutcomeDto);
        }

    }
}
