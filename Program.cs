using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security;

class Program
{
    static void Main()
    {
        // SharePoint site URL
        string siteUrl = "http://YourSPSiteUrl";

        // Credentials (adjust accordingly)
        string username = "kattheuser@domain123.com";
        string password = "Pas55wordsad";
        SecureString securePassword = new NetworkCredential("", password).SecurePassword;

        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        // Create SharePoint context
        using (ClientContext context = new ClientContext(siteUrl))
        {
            context.Credentials = new SharePointOnlineCredentials(username, securePassword);

            // Create a new list
            ListCreationInformation listCreationInfo = new ListCreationInformation
            {
                Title = "YourListTitle",
                TemplateType = (int)ListTemplateType.GenericList
            };

            List newList = context.Web.Lists.Add(listCreationInfo);
            context.ExecuteQuery();

            // Add default columns to the list
            CreateTextField(context, newList, "TextColumn", "Your Default Text Value");
            CreateDateTimeField(context, newList, "DateColumn", DateTime.Now);
            CreateChoiceField(context, newList, "ChoiceColumn", "Choice1", "Choice2", "Choice3");
            CreateNumberField(context, newList, "NumberColumn", 42);
            CreateBooleanField(context, newList, "BooleanColumn", true);

            Console.WriteLine("SharePoint list created successfully with default columns!");
        }
    }

    static void CreateTextField(ClientContext context, List list, string columnName, string defaultValue)
    {
        Field field = list.Fields.AddFieldAsXml(
            $"<Field Type='Text' DisplayName='{columnName}' StaticName='{columnName}' />",
            true,
            AddFieldOptions.DefaultValue);

        field.DefaultValue = defaultValue;
        field.Update();
        context.ExecuteQuery();
    }

    static void CreateDateTimeField(ClientContext context, List list, string columnName, DateTime defaultValue)
    {
        Field field = list.Fields.AddFieldAsXml(
            $"<Field Type='DateTime' DisplayName='{columnName}' StaticName='{columnName}' Format='DateTime' />",
            true,
            AddFieldOptions.DefaultValue);

        field.DefaultValue = defaultValue.ToString("yyyy-MM-ddTHH:mm:ssZ");
        field.Update();
        context.ExecuteQuery();
    }

    static void CreateChoiceField(ClientContext context, List list, string columnName, params string[] choices)
    {
        // Create a choice field
        FieldChoice choiceField = (FieldChoice)list.Fields.AddFieldAsXml(
            $"<Field Type='Choice' DisplayName='{columnName}' StaticName='{columnName}' Format='Dropdown' />",
            true,
            AddFieldOptions.DefaultValue);

        // Populate the choices
        choiceField.Choices = choices;

        // Set default value to the first choice
        choiceField.DefaultValue = choices[0];

        // Update and execute the query
        choiceField.Update();
        context.ExecuteQuery();
    }

    static void CreateNumberField(ClientContext context, List list, string columnName, double defaultValue)
    {
        Field field = list.Fields.AddFieldAsXml(
            $"<Field Type='Number' DisplayName='{columnName}' StaticName='{columnName}' />",
            true,
            AddFieldOptions.DefaultValue);

        field.DefaultValue = defaultValue.ToString();
        field.Update();
        context.ExecuteQuery();
    }

    static void CreateBooleanField(ClientContext context, List list, string columnName, bool defaultValue)
    {
        Field field = list.Fields.AddFieldAsXml(
            $"<Field Type='Boolean' DisplayName='{columnName}' StaticName='{columnName}' />",
            true,
            AddFieldOptions.DefaultValue);

        field.DefaultValue = defaultValue ? "1" : "0"; // SharePoint uses "1" for true and "0" for false
        field.Update();
        context.ExecuteQuery();
    }
}
