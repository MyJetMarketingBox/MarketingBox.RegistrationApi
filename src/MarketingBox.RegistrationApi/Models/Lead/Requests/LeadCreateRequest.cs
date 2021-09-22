using System.Collections.Generic;
using Destructurama.Attributed;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MarketingBox.RegistrationApi.Models.Lead.Requests
{
    /// <summary>
    /// Operation filter to add the requirement of the custom header
    /// </summary>
    //public class MyHeaderFilter : IOperationFilter
    //{
    //    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    //    {
    //        if (operation.Parameters == null)
    //            operation.Parameters = new List<OpenApiParameter>();

    //        operation.Parameters.Add(new NonBodyParameter
    //        {
    //            Name = "MY-HEADER",
    //            In = "header",
    //            Type = "string",
    //            Required = true // set to false if this is optional
    //        });
    //    }
    //}

    public class LeadCreateRequest
    {
        #region Personal info
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string FirstName { get; set; }
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string LastName { get; set; }
        [LogMasked(PreserveLength = true)]
        public string Password { get; set; }
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Email { get; set; }
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Phone { get; set; }
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Ip { get; set; }
        #endregion
        
        #region Additional parameters
        public string So { get; set; }
        public string Sub { get; set; }
        public string Sub1 { get; set; }
        public string Sub2 { get; set; }
        public string Sub3 { get; set; }
        public string Sub4 { get; set; }
        public string Sub5 { get; set; }
        public string Sub6 { get; set; }
        public string Sub7 { get; set; }
        public string Sub8 { get; set; }
        public string Sub9 { get; set; }
        public string Sub10 { get; set; }
        #endregion  
    }
}
