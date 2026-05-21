namespace LeaveFlowHR.Api.Common.Utilites;

public static class DateUtility
{

    public static int DifferenceInBusinessDays(DateOnly startDate, DateOnly endDate)
    {
        int businessDays = 0;

        // Include the end date in the calculation
        endDate = endDate.AddDays(1);

        for (DateOnly date = startDate; date < endDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday &&
                date.DayOfWeek != DayOfWeek.Sunday)
            {
                businessDays++;
            }
        }

        return businessDays;
    }
}