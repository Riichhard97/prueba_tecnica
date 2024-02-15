using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Nexu.Shared.AspNetCore.Response
{
    public class IdentityResultCustom
    {
        private string message;
        public bool Succeeded { get; }
        public object Data { get; }
        public IEnumerable<IdentityError> Errors { get; }
        public string Message
        {
            get
            {
                if (!Succeeded && string.IsNullOrEmpty(this.message)) return Errors.ToString();
                return message;
            }
        }

        public IdentityResultCustom() { }

        public IdentityResultCustom(bool success, string message, object data, IEnumerable<IdentityError> errors)
        {
            Succeeded = success;
            this.message = message;
            Data = data;
            Errors = errors;
        }

        public static IdentityResultCustom BadRequest(params IdentityError[] errors)
        {
            return new IdentityResultCustom(false, string.Empty, null, errors);
        }

        public static IdentityResultCustom BadRequest(string code, string description)
        {
            //Exception exception = new Exception(code);
            //throw new InvalidOperationException("User alredy register");
            return new IdentityResultCustom(false, string.Empty, null, new IdentityError[] {
                        new IdentityError{
                            Code = code,
                            Description = description
                        }
                    });
        }

        public static IdentityResultCustom Ok(object data, string message = "OK")
        {
            return new IdentityResultCustom(true, message, data, null);
        }
    }
}
