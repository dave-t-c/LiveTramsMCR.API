using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TfGM_API_Wrapper.Models.Services;

/// <summary>
/// Converts Unformatted services, into a formatted form, by removing null or empty services
/// and re-formatting the messages to be clearer.
/// </summary>
public class ServiceFormatter
{
    /// <summary>
    ///     Converts a list of unformatted services into a Formatted services object.
    /// </summary>
    /// <param name="unformattedServices">List of Unformatted services to format.</param>
    /// <returns>Formatted Services: Combined result of each of the unformatted services.</returns>
    public FormattedServices FormatServices(List<UnformattedServices> unformattedServices)
    {
        if (unformattedServices == null) throw new ArgumentNullException(nameof(unformattedServices));
        var formattedServices = new FormattedServices();
        foreach (var service in unformattedServices)
        {
            // This currently appears to be the easiest way to create information for all four possible trams.
            AddTram(formattedServices, new Tram(service.Dest0, service.Carriages0, service.Status0,
                service.Wait0));
            AddTram(formattedServices, new Tram(service.Dest1, service.Carriages1, service.Status1,
                service.Wait1));
            AddTram(formattedServices, new Tram(service.Dest2, service.Carriages2, service.Status2,
                service.Wait2));
            AddTram(formattedServices, new Tram(service.Dest3, service.Carriages3, service.Status3,
                service.Wait3));

            FormatMessage(formattedServices, service.MessageBoard);
        }

        return formattedServices;
    }

    /// <summary>
    /// Converts unformatted services to Departure board services
    /// </summary>
    /// <param name="unformattedServices"></param>
    /// <returns></returns>
    public FormattedDepartureBoardServices FormatDepartureBoardServices(List<UnformattedServices> unformattedServices)
    {
        var formattedDepartureBoardServices = new FormattedDepartureBoardServices();
        foreach (var service in unformattedServices)
        {
            AddTram(formattedDepartureBoardServices, new Tram(service.Dest0, service.Carriages0, service.Status0,
                service.Wait0));
            AddTram(formattedDepartureBoardServices, new Tram(service.Dest1, service.Carriages1, service.Status1,
                service.Wait1));
            AddTram(formattedDepartureBoardServices, new Tram(service.Dest2, service.Carriages2, service.Status2,
                service.Wait2));
            AddTram(formattedDepartureBoardServices, new Tram(service.Dest3, service.Carriages3, service.Status3,
                service.Wait3));
        }

        return formattedDepartureBoardServices;
    }

    /// <summary>
    ///     Adds a tram to the formatted services if the destination string is not null or empty.
    ///     This assumes that if the destination is null or empty, the other fields such as carriages will
    ///     also be null or empty.
    /// </summary>
    /// <param name="formattedServices">Formatted service to add tram to</param>
    /// <param name="tram">Tram to add to formatted services</param>
    private static void AddTram(FormattedServices formattedServices, Tram tram)
    {
        if (string.IsNullOrEmpty(tram.Destination)) return;
        formattedServices.AddService(tram);
    }


    /// <summary>
    /// Adds a tram to the formatted departure board services if the tram is
    /// not null and the destination is not null or empty
    /// </summary>
    /// <param name="formattedServices"></param>
    /// <param name="tram"></param>
    private static void AddTram(FormattedDepartureBoardServices formattedServices, Tram tram)
    {
        if (string.IsNullOrEmpty(tram.Destination)) return;
        formattedServices.AddService(tram);
    }

    /// <summary>
    ///     Formats the message board value from the unformatted service.
    ///     Removes expected caret chars, and ignores empty or null messages.
    /// </summary>
    /// <param name="formattedServices">Services to add message to</param>
    /// <param name="message">Message to format</param>
    private static void FormatMessage(FormattedServices formattedServices, string message)
    {
        if (string.IsNullOrEmpty(message) || message == "<no message>") return;
        //Replace the caret chars with spaces for the centre.
        //This could create an excess space at the start, so run TrimStart.
        message = Regex.Replace(message, @"\^J\^F0|\^F0", " ").TrimStart();
        formattedServices.AddMessage(message);
    }
}