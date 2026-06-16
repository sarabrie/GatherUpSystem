using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.Interfaces;
using GatherUp.Core.DO.Polls;
using GatherUp.Core.DO;

namespace GatherUp.BL.Services
{
    public class PollService
    {
        private readonly IRepository<Poll> _pollRepo;
        private readonly IRepository<Event> _eventRepo;

        public PollService(IRepository<Poll> pollRepo, IRepository<Event> eventRepo)
        {
            _pollRepo = pollRepo;
            _eventRepo = eventRepo;
        }

        public IEnumerable<Poll> GetEventPolls(int eventId)
        {
            Event ev = _eventRepo.GetById(eventId);
            if (ev == null || ev.PollIds == null)
                return Enumerable.Empty<Poll>();

            IEnumerable<Poll> allPolls = _pollRepo.GetAll();
            return allPolls.Where(p => ev.PollIds.Contains(p.Id));
        }
         public Poll GetPollById(int pollId)
        {
            return _pollRepo.GetById(pollId);
        }

        public IEnumerable<PollQuestion> GetAllQuestionsFromAllPolls()
        {
            return _pollRepo.GetAll()
                .SelectMany(poll => poll.Questions);
        }

        public string GetMostVotedOption(int pollId, int questionId)
        {
            var poll = _pollRepo.GetById(pollId);
            if (poll == null) return "הסקר לא נמצא";

            var question = poll.Questions.FirstOrDefault(q => q.QuestionId == questionId);
            if (question == null || question.ParticipantVotes == null || !question.ParticipantVotes.Any())
            {
                return "אין עדיין הצבעות לשאלה זו";
            }

            var mostVotedOptionIndex = question.ParticipantVotes
                .GroupBy(vote => vote.Value)
                .OrderByDescending(group => group.Count())
                .First()
                .Key;

            if (mostVotedOptionIndex >= 0 && mostVotedOptionIndex < question.Options.Count)
            {
                return question.Options[mostVotedOptionIndex];
            }

            return "נמצאה הצבעה לאופציה לא תקינה";
        }
    }
}