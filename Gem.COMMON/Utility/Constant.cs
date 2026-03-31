using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Reflection;
namespace Gem.COMMON.Utility
{
    public static class Constant
    {

    }

    public static class Messages
    {
        //Reponse messages related to Image and File Analysis
        public const string ANALYSIS_SUCCESS = "Analysis done successfully";
        // Response message for Attachment
        public const string ATTACHMENT_DELSUCCESS = "File deleted successfully";
        public const string ATTACHMENT_NOTFOUND = "File not found";
        public const string ATTACHMENT_ERROR = "An error occurred while deleting the attachement:";
        public const string ATTACHMENT_SUCCESS = "Attachments saved successfully";
        public const string NO_ATTACHMENT = "No attachment provided";
        public const string ATTACHMENT_INVALID = "Attachment must have either File or Data.";
        public const string ATTACHMENT_TYPE_NOT_ALLOWED = "Attachement Type is not allowed";
        public const string ATTACHMENT_SIZE_EXCEEDED = "Attachement size should be less than 50mb";

        //Response Messages for User
        public const string ALREADY_USER = "User already exist. Please log in";
        public const string USER_CREATE_SUCCESS = "User Created Successfully";
        public const string USER_INVALID_CREDS = "Please enter valid credentials";
        public const string USER_DELETED = "User not found. Create an account to begin!";
        public const string USER_NOTFOUND = "User not found";
        public const string LOGIN_SUCCESSFULLY = "User login successfully";
        public const string LOGIN_FAILED = "User login failed";
        public const string LOGOUT_SUCCESSFULLY = "Logout successfully";
        public const string USER_NOTASSOCIATED = "No User associated with this Email";
        public const string USER_INVALID = "Invalid User";
        public const string USER_DELSUCCESS = "User deleted successfully";
        public const string ACCOUNT_VERIFIED = "Account verified successfullly";
        public const string USER_UPDATE_SUCCESS = "User update successfully";
        public const string USER_FETCH_SUCCESS = "User Fetch successfully";
        public const string USER_PHONE_EXIST = "Phone Already exist.Please enter Another phone number";
        public const string USER_EMAIL_EXIST = "Emaill Already exist.Please enter Another phone number";
        public const string USER_PHONE_EXIST_NOCON = "Phone number already exists but not confirmed.Please check you sms and confirm account.";
        public const string USER_Email_EXIST_NOCON = "Email already exists but not confirmed. Please check your email and confirm your account.";
        public const string OTP_SEND = "Please check email/phone otp has been sent";
        public const string UNAUTHORIZED_ACCESS = "UnAuthorized access";

        //Response messages related to otp
        public const string OTP_VERIFIED_SUCCESS = "Otp verified successfully";
        public const string INVALID_OTP = "Invalid otp. Please enter otp again";

        //Response Messages for password related 
        public const string INVALID_PASSWORD = "Password you have entered is invalid.";
        public const string PASSWORD_CHANGE_SUCCESS = "Password change successfully.";
        public const string RESET_PASSWORD_SUCCESS = "Password reset successfully.";

        // Response messages related to profile
        public const string PROFILE_UPDATE_SUCEESS = "Profile update Successfully";

        //Response messages for email and phone number related
        public const string EMAIL_ALREADY_CONFIRMED = "Email is all ready confirmed";
        public const string EMAIL_VERIFIED_SUCCESS = "Email is verified successfully";
        public const string EMAIL_SEND_SUCCESS = "Email send successfully";
        public const string EMAIL_SEND_FAILED = "Failed to send email";
        public const string EMAIL_UNVERIFIED = "Email is not verified.Please check your email and verify.";
        public const string PHONE_UNVERIFIED = "Phone Number is not verified. Please check your messages and verify your phone Number";
        public const string EMAIL_ERROR = "An error occur while sending email";

        // Response for Roles
        public const string ROLE_NOTFOUND = "User role is not found.Please contact admin";

        //Response related to errors
        public const string ERROR_OCCUR = "Something went wrong Please try again";
        public const string SERVER_ERROR = "Internal server error";

        // Response related to logging error in database or file
        public const string EXCEPTION_LOGSUCCESS = "Exception log in database successfully";
        public const string EXCEPTION_FETCHSUCCESS = "Exception log fetch successfully";
        public const string EXCEPTION_NOFOUND = "Exception log not found";

        // Response messages relate to otp
        public const string OTP_SENT_SUCCESS = "Otp send successfully";
        public const string OTP_SENT_FAILED = "Failed to send otp";
        public const string OTP_PHONE_SUCCESS = "Otp send on your phone please verify";
        public const string OTP_EMAIL_SUCCESS = "Otp send on your email please verify";

        //Response message for dashboard
        public const string DASHBOARD_SUCCESS = "Dashbord loaded successfuly";
        public const string SUCCESS = "SUCCESS";

        // Response message related to Image and Documents
        public const string IMAGE_DELSUCCESS = "Deleted successfully";
        public const string IMAGE_NOFOUND = "Not found";

        //Response message realted to Contact us
        public const string CONTACT_ADD_SUCCESS = "Query submitted successfully";
        public const string CONTACT_UPDATE_SUCCESS = "Query status updated successfully";
        public const string CONTACT_DELSUCCESS = "Query deleted successfully";

        //Response message realted to NOTIFICATIONS
        public const string NOTIFICATION_SAVE_SUCCESS = "Query submitted successfully";
        public const string NOTIFICATION_READ_SUCCESS = "Notification read successfully";

        //Others 
        public const string INVALID_DATE_RANGE = "Date range is invalid.";
        public const string INVALID_TOKEN = "Invalid token";
        public const string INVALID_ID = "Invalid ID";


        //RESPONSE MESSAGES RELATED TO INVALID REQUEST
        public const string INVALID_REQUEST = "Request is invalid!";

        //RESPONSE MESSAGES RELATED TO SUBSCRIPTION PLAN
        public const string SUBSCRIPTION_ADDSUCCESS = "Subscription plan added succcessfully";
        public const string SUBSCRIPTION_ALREADYEXIST = "A subscription plan already exist";
        public const string SUBSCRIPTION_UPDATESUCCESS = "Subscription plan updated succcessfully";
        public const string SUBSCRIPTION_DELETESUCCESS = "Subscription plan deleted successfuly";
        public const string SUBSCRIPTION_NOTFOUND = "Subscription plan not found";
        public const string SUBSCRIPTION_FETCHSUCCESS = "Subscription plan Fetch found";
        public const string SUBSCRIPTION_ISINUSE = "This subscription plan is currently in use and cannot be deleted.";

        //RESPONSE MESSAGES RELATED TO GENERAL SETTINGS
        public const string SETTING_NOTFOUND = "Settings not found";
        public const string SETTING_UPSUCCESS = "Setting update successfully";
        public const string SETTING_CREATESUCCESS = "Setting added successfully";
        public const string SETTING_ALREADYEXIST = "Setting already exist";
        public const string SETTING_FETCHSUCCESS = "Setting fetch successfully";

        //Response MESSAGES RELATED TO PLAN PURCHASE
        public const string PLAN_UPGRADESUCCESS = "Plan upgraded successfully";

        //Response MESSAGES RELATED TO SUBSCRIPTION PAYMENT
        public const string Payment_Success = "Payment Successful";

        //RESPONSE MESSAGES RELATED TO  NOTIFICAION USAGE
        public const string NOTIFICAIONUSAGE_ADDSUCCESS = "Usage added successfully";

        // RESPONSE MESSAGES RELATED TO ACCES
        public const string MODULE_ACCESS_GRANTED = "Access granted successsfully";
        public const string MODULE_ACCESS_DENIED = "Access denied";
        public const string QUOTA_AVAILABLE = "Quota available";
        public const string QUOTA_EXCEEDED = "Quota exceeded";


        public const string UNAUTHORIZED = "You are not authorized to perform this action.";
        public static readonly string MESSAGE_SUCCESS;
        public static readonly string TOKEN_USAGE_SUCCESS;
    }

    public static class VerificationType
    {
        public const string PHONE_REG = "phone_registration";
        public const string EMAIL_REG = "email_registration";
        public const string EMAIL_CH = "email_change";
        public const string PHONE_CH = "phone_change";
        public const string EMAIL_FGT = "email_forgot_password";
        public const string PHONE_FGT = "phone_forgot_password";
        public const string LOGIN_OTP_EMAIL = "login_with_otp_email";
        public const string LOGIN_OTP_PHONE = "login_with_otp_phone";
    }

    public static class Subject
    {
        public const string VERIFICATION = "Verify Email Address";
        public const string RESET = "Reset Password";
        public const string WELCOME = "Welcome To Rentkagaz";
        public const string LOGIN = "Login To Rentkagaz";
        public const string EMAIL_CHANGED = "Email Change Request";
    }

    public static class EmailTemplate
    {
        public const string VERIFY_EMAIL_TEMPLATE = "verify.html";
        public const string FORGOT_PASSWORD_EMAIL_TEMPLATE = "forget password.html";
        public const string WELCOME_EMAIL_TEMPLATE = "welcome.html";
        public const string TNT_SUBPROP_ASNMT_ET = "assign-tenant-email.html";
        public const string INV_STS_ET = "invoice-status.html";
        public const string PYMT_ADDED = "payment-added.html";
        public const string PYMT_STS_ET = "payment-status.html";
        public const string RMV_TNT_ET = "remove-tenant.html";
        public const string LOG_ET = "login.html";
        public const string EMAIL_CHANGE = "email_change.html";
    }

    public static class NotificationType
    {
        public const string INVOICEGENERATION_NOTIFICAION = "InvoiceGenerationNotification";
        public const string PAYMENTADD_NOTIFICAION = "Payment Add Notification";
        public const string PAYMENTSTATUSCHANGE_NOTIFICAION = "Payment Status Changed Notification";
    }

    public static class NotificationMessage
    {
        public const string INVOICE_GENERATED = "An invoice has been generated for you. Please review and complete the payment.";
        public const string PAYMENT_ADDED = "A payment has been added to your invoice. Please review the details.";
        public const string PAYMENT_STATUS_CHANGED = "The status of your payment for invoice has been updated. Please review the changes.";
    }

    public static class NotificationTitle
    {
        public const string INVOICEGENERATION_TITLE = "New Invoice Generated";
        public const string PAYMENTADD_TITLE = "Payment Received";
        public const string PAYMENTSTATUSCHANGE_TITLE = "Payment Status Changed";
    };
}