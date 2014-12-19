using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Guestbook
{
    public class CreateGuestbookEntry : BaseCreateEntityCommand<GuestbookEntry>, IDefineCommand
    {
        public string Text { get; [UsedImplicitly] set; }
    }

    [UsedImplicitly]
    public class ValidateCreateGuestbookEntryCommand : AbstractValidator<CreateGuestbookEntry>
    {
        public ValidateCreateGuestbookEntryCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Text)
                .NotEmpty()
                .WithName("Message")
            ;
        }
    }

    [UsedImplicitly]
    public class HandleCreateGuestbookEntryCommand : IHandleCommand<CreateGuestbookEntry>
    {
        private readonly IWriteEntities _entities;

        public HandleCreateGuestbookEntryCommand(IWriteEntities entities)
        {
            _entities = entities;
        }

        public async Task Handle(CreateGuestbookEntry command)
        {
            var entity = new GuestbookEntry
            {
                Text = command.Text,
            };
            _entities.Create(entity);

            command.CreatedEntity = entity;

            if (command.Commit)
            {
                await _entities.SaveChangesAsync();
            }
        }
    }
}
