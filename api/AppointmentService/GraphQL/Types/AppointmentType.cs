using AppointmentService.Models;
using GraphQL.Types;

namespace AppointmentService.GraphQL.Types;

public sealed class AppointmentType : ObjectGraphType<AppointmentModel>
{
    public AppointmentType()
    {
        Field(x => x.Id);
        Field(x => x.StartTime);
        Field(x => x.EndTime);
        Field(x => x.Title);
        Field(x => x.Description);
        Field(x => x.Status);
        Field(x => x.DoctorId);
        Field(x => x.PatientId);
        Field(x => x.CreatedBy);
    }
}