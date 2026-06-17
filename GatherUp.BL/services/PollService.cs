using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.Interfaces;
using GatherUp.Core.DO;
using GatherUp.Core.DO.Polls;

namespace GatherUp.BL.Services
{
    public class PollService
    {
        private readonly IRepository<Event> _eventRepo;
        private readonly IRepository<Poll> _pollRepo;

        public PollService(IRepository<Event> eventRepo, IRepository<Poll> pollRepo)
        {
            _eventRepo = eventRepo;
            _pollRepo = pollRepo;
        }

        public IEnumerable<Poll> GetEventPolls(int eventId)
        {
            Event ev = _eventRepo.GetById(eventId);
            if (ev == null || ev.PollIds == null)
                return Enumerable.Empty<Poll>();

            return _pollRepo.GetAll().Where(p => ev.PollIds.Contains(p.Id));
        }

        public bool IsPollValidAndActive(int pollId)
        {
            Poll poll = _pollRepo.GetAll().FirstOrDefault(p => p.Id == pollId);

            return poll != null && poll.Questions != null && poll.Questions.Any();
        }

        public Dictionary<string, double> CalculateQuestionResultsPercentages(int pollId, int questionId)
        {
            Poll poll = _pollRepo.GetAll().FirstOrDefault(p => p.Id == pollId);
            if (poll == null || poll.Questions == null)
                return new Dictionary<string, double>();

            var question = poll.Questions.FirstOrDefault(q => q.QuestionId == questionId);

            if (question == null || question.Options == null || !question.Options.Any())
                return new Dictionary<string, double>();

            int totalVotes = question.ParticipantVotesXml != null ? question.ParticipantVotesXml.Length : 0;

            if (totalVotes == 0)
                return question.Options.ToDictionary(optionText => optionText, percentage => 0.0);

            return question.Options
                .Select((optionText, index) => new
                {
                    Text = optionText,
                    VoteCount = question.ParticipantVotesXml.Count(v => v.ChosenOptionIndex == index)
                })
                .ToDictionary(
                    x => x.Text,
                    x => Math.Round((double)x.VoteCount / totalVotes * 100, 2) 
                );
        }
    }
}