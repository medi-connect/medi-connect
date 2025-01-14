using AppointmentService.GraphQL.Queries;

namespace AppointmentService.GraphQL.Schema;

public class AppointmentSchema : global::GraphQL.Types.Schema
{
    public AppointmentSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Query = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<MainQuery>();
    }
}