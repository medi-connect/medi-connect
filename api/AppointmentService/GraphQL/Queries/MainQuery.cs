using AppointmentService.GraphQL.Types;
using AppointmentService.Utils;
using GraphQL;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.GraphQL.Queries;

public sealed class MainQuery : ObjectGraphType
{
    public MainQuery(ApplicationDbContext dbContext)
    {
        Field<ListGraphType<AppointmentType>>("appointments")
            .ResolveAsync(_ =>
            {
                var appointments = dbContext.Appointment.ToList();
                return Task.FromResult<object?>(appointments);
            });

        Field<AppointmentType>("appointment")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<IntGraphType>> {Name = "id"}))
                .Resolve(context =>
                {
                    var id = context.GetArgument<int>("id");
                    var appointment = dbContext.Appointment.FirstOrDefaultAsync(x => x.Id == id).Result;
                    return appointment;
                });
    }
}