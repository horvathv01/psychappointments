using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service;

public class SessionService : ISessionService
{
    private readonly PsychAppointmentContext _context;
    private readonly IUserService _userService;
    private readonly ILocationService _locationService;
    private readonly ISlotService _slotService;
    
    public SessionService(
        PsychAppointmentContext context,
        IUserService userService,
        ILocationService locationService,
        ISlotService slotService
    )
    {
        _context = context;
        _userService = userService;
        _locationService = locationService;
        _slotService = slotService;
    }
    
    public async Task<bool> AddSession(SessionDTO session)
    {
        //long id = await _context.Slots.CountAsync() + 1;
        //session.Id = id;
        session.Date = DateTime.SpecifyKind(session.Date, DateTimeKind.Utc);
        session.Start = new DateTime(session.Date.Year, session.Date.Month, session.Date.Day, session.Start.Hour, session.Start.Minute, session.Start.Second);
        session.End = new DateTime(session.Date.Year, session.Date.Month, session.Date.Day, session.End.Hour, session.End.Minute, session.End.Second);
        
        session.Start = DateTime.SpecifyKind(session.Start, DateTimeKind.Utc);
        session.End = DateTime.SpecifyKind(session.End, DateTimeKind.Utc);

        //find slots from same day, location and psychologist to avoid overlaps
        var sameDaysSessions = await _context.Sessions
            .Where(ses => ses.Date == session.Date && ses.Location.Id == session.LocationId && ses.Psychologist.Id == session.PsychologistId)
            .ToListAsync();
        
        foreach (var sameDaysSession in sameDaysSessions)
        {
            //if slot overlaps with another, it cannot be added
            if (Overlap(session, new SessionDTO(sameDaysSession))) return false;
        }
        
        if (session.PsychologistId == null || session.LocationId == null)
        {
            Console.WriteLine($"Psychologist id, location id and slot id in a sessionDTO are required for creating a session.");
            return false;
        }
        
        try
        {
            var psychologist = (Psychologist?)await _userService.GetUserById((long)session.PsychologistId);
            var location = await _locationService.GetLocationById((long)session.LocationId);
            
            if (psychologist == null || location == null)
            {
                Console.WriteLine($"Psychologist or location not found when trying to add a session.");
                return false;
            }
            
            //if slot id is not provided, we create a slot just for this session
            if (session.SlotId == null)
            {
                //create new slot && calculate slot length
                int slotLength = (int)(session.End - session.Start).TotalMinutes;
                var newSlot = new SlotDTO(psychologist, location, session.Date, session.Start, session.End, slotLength, 0, false, new List<Session>());
                //add new slot to DB
                await _slotService.AddSlot(newSlot, false);
            }

            Slot? slot;
            if (session.SlotId == null)
            {
                //gets slot that we just added by start&end
                slot = (await _slotService.GetSlotsByPsychologistLocationAndDates(psychologist,
                    location, session.Start, session.End)).FirstOrDefault();
            }
            else
            {
                slot = await _slotService.GetSlotById((long)session.SlotId);
            }
            
            //if slot is null, something went wrong --> should be handled
            if (slot == null)
            {
                Console.WriteLine($"Slot not found when trying to add a session.");
                return false;
            }
            
            Psychologist? partnerPsychologist = session.PsychologistId == null
                ? null
                : (Psychologist?)await _userService.GetUserById((long)session.PsychologistId);
            
            int price = session.Price ?? 0;


            
            SessionFrequency frequency = SessionFrequency.None; 
            Enum.TryParse(session.Frequency, out frequency);
            Client? client = null;
            if (session.ClientId != null)
            {
                client = (Client?)await _userService.GetUserById((long)session.ClientId);
                session.Blank = false;
            }
            else
            {
                session.Blank = true;
            }

            Session newSession = new Session(psychologist, location, session.Date, session.Start, session.End, slot, price, session.Blank,session.Description, frequency, client, partnerPsychologist);
            
            await _context.Sessions.AddAsync(newSession);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Slot could not be added.");
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<Session?> GetSessionById(long id)
    {
        return await _context.Sessions
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .FirstOrDefaultAsync(ses => ses.Id == id);
    }

    public async Task<IEnumerable<Session>> GetAllSessions()
    {
        return await _context.Sessions
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .ToListAsync();
    }

    public async Task<IEnumerable<Session>> GetNonBlankSessions()
    {
        return await _context.Sessions
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .Where(ses => !ses.Blank)
            .ToListAsync();
    }

    public async Task<List<Session>> GetListOfSessions(List<long> ids)
    {
        return await _context.Sessions
            .Where(ses => ids.Contains(ses.Id))
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .ToListAsync();
    }

    public async Task<IEnumerable<Session>> GetSessionsByLocation(Location location, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        if (startOfRange == null || endOfRange == null)
        {
            
            return await _context.Sessions
                .Where(ses => ses.Location.Equals(location))
                .Include(ses => ses.Psychologist)
                .Include(ses => ses.PartnerPsychologist)
                .Include(ses => ses.Location)
                .Include(ses => ses.Slot)
                .Include(ses => ses.Client)
                .ToListAsync();
        }
        
        return await _context.Sessions
            .Where(ses => ses.Location.Equals(location) && ses.Date >= startOfRange &&
                          ses.Date <= endOfRange)
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .ToListAsync();
    }

    public async Task<IEnumerable<Session>> GetSessionsByPsychologist(Psychologist psychologist, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        if (startOfRange == null || endOfRange == null)
        {
            return await _context.Sessions
                .Where(ses => ses.Psychologist.Equals(psychologist))
                .Include(ses => ses.Psychologist)
                .Include(ses => ses.PartnerPsychologist)
                .Include(ses => ses.Location)
                .Include(ses => ses.Slot)
                .Include(ses => ses.Client)
                .ToListAsync();
        }
        
        return await _context.Sessions
            .Where(ses => ses.Psychologist.Equals(psychologist) && ses.Date >= startOfRange &&
                          ses.Date <= endOfRange)
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Session>> GetSessionsByPsychologistLocationAndDates(Psychologist psychologist, Location location, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        if (startOfRange == null || endOfRange == null)
        {
            return await _context.Sessions
                .Where(ses => ses.Location.Equals(location) &&  ses.Psychologist.Equals(psychologist))
                .Include(ses => ses.Psychologist)
                .Include(ses => ses.PartnerPsychologist)
                .Include(ses => ses.Location)
                .Include(ses => ses.Slot)
                .Include(ses => ses.Client)
                .ToListAsync();
        }
        
        return await _context.Sessions
            .Where(ses => ses.Location.Equals(location) &&  ses.Psychologist.Equals(psychologist) && ses.Date >= startOfRange &&
                         ses.Date <= endOfRange)
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .ToListAsync();
    }

    public List<Session> GetSessionsByClient(Client client, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        return client.Sessions.Where(ses =>
            ses.Start >= startOfRange
            && ses.End <= endOfRange
        ).ToList();
    }

    public async Task<List<Session>> GetSessionsByManager(Manager manager, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        if (startOfRange == null || endOfRange == null)
        {
            return await _context.Sessions
                .Where(ses => ses.Location.Managers.Contains(manager))
                .ToListAsync();
        }

        return await _context.Sessions
            .Where(ses => ses.Location.Managers.Contains(manager) && ses.Start >= startOfRange &&
                         ses.End <= endOfRange)
            .ToListAsync();
    }

    public async Task<List<Session>> GetSessionsByDate(DateTime startOfRange, DateTime endOfRange)
    {
        return await _context.Sessions
            .Where(ses => ses.Date >= startOfRange && ses.Date <= endOfRange)
            .ToListAsync();
    }

    public async Task<bool> UpdateSession(long id, SessionDTO session)
    {
        try
        {
            var original = await GetSessionById(id);
            if (original == null)
            {
                Console.WriteLine($"Session {id} not found in DB.");
                return false;
            }
            
            //find slots from same day, location and psychologist to avoid overlaps
            var sameDaysSessions = await _context.Sessions
                .Where(ses => ses.Date == session.Date && ses.Location.Id == session.LocationId && ses.Psychologist.Id == session.PsychologistId)
                .ToListAsync();
        
            foreach (var sameDaysSession in sameDaysSessions)
            {
                //if slot overlaps with another, it cannot be added
                if (Overlap(session, new SessionDTO(sameDaysSession)))
                {
                    Console.WriteLine($"Updating session {id} is not possible because it would overlap with session {sameDaysSession.Id}.");
                    return false;
                }
            }

            if (session.PsychologistId == null || session.LocationId == null || session.SlotId == null)
            {
                Console.WriteLine($"Psychologist id, location id and slot id in a sessionDTO are required for updating session.");
                return false;
            }
            //crucial parameters
            Psychologist? psychologist = (Psychologist?)await _userService.GetUserById((long)session.PsychologistId);
            Location? location = await _locationService.GetLocationById((long)session.LocationId);
            Slot? slot = await _slotService.GetSlotById((long)session.SlotId);
            
            if (psychologist == null || location == null || slot == null)
            {
                Console.WriteLine($"Psychologist, location or slot not found when trying to update a session.");
                return false;
            }
            
            //non-crucial parameters
            Psychologist? partnerPsychologist = session.PsychologistId == null
                ? null
                : (Psychologist?)await _userService.GetUserById((long)session.PsychologistId);
            Client? client = null;
            if (session.ClientId != null)
            {
                client = (Client?)await _userService.GetUserById((long)session.ClientId);
                session.Blank = false;
            }
            else
            {
                session.Blank = true;
            }

            int price = session.Price ?? 0;
            SessionFrequency frequency = SessionFrequency.None; 
            Enum.TryParse(session.Frequency, out frequency);

            original.PsychologistId = psychologist.Id;
            original.Psychologist = psychologist;
            original.PartnerPsychologistId = partnerPsychologist?.Id ?? 0;
            original.PartnerPsychologist = partnerPsychologist;
            original.Blank = session.Blank;
            original.LocationId = location.Id;
            original.Location = location;
            original.Date = DateTime.SpecifyKind(session.Date, DateTimeKind.Utc);
            original.Start = DateTime.SpecifyKind(session.Start, DateTimeKind.Utc);
            original.End = DateTime.SpecifyKind(session.End, DateTimeKind.Utc);
            original.ClientId = client?.Id ?? 0;
            original.Client = client;
            original.Price = price;
            original.Frequency = frequency;
            original.SlotId = slot.Id;
            original.Slot = slot;
            original.SlotId = slot.Id;
            original.Description = session.Description;

            _context.Update(original);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> DeleteSession(long id)
    {
        try
        {
            var session = await GetSessionById(id);
            if (session == null) return false;
            _context.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public bool Overlap(SessionDTO session1, SessionDTO session2)
    {
        if (session1.Date != session2.Date)
        {
            return false;
        }

        List<SessionDTO> sessions = new List<SessionDTO>(){ session1, session2 };  
        sessions.Sort((ses1, ses2) => ses1.Start.CompareTo(ses2.Start));
        
        if (sessions[0].End > sessions[1].Start)
        {
            return true;
        }

        return false;
    }
}