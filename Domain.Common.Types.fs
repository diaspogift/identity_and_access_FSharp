namespace IdentityAndAcccess.CommonDomainTypes



///Tenant related types definition
/// 
/// 
type TenantId = private TenantId of string
type TenantName = private TenantName of string
type TenantDescription = private TenantDescription of string
type RegistrationInvitationId = private RegistrationInvitationId of string
type RegistrationInvitationDescription = private RegistrationInvitationDescription of string


///User related types definition
/// 
/// 
type UserId = private UserId of string
type Username = private Username of string
type Password = private Password of string
type FirstName = private FirstName of string
type MiddleName = private MiddleName of string
type LastName = private LastName of string
type EmailAddress = private EmailAddress of string
type PostalAddress = private PostalAddress of string
type Telephone = private Telephone of string
type UserDescriptorId = private UserDescriptorId of string


///Role related types definition
/// 
/// 
type RoleId = private RoleId of string
type RoleName = private RoleName of string
type RoleDescription = private RoleDescription of string


///Group related types definition
/// 
/// 
type GroupId = private GroupId of string
type GroupName = private GroupName of string
type GroupDescription = private GroupDescription of string
type GroupMemberId = private GroupMemberId of string
type GroupMemberName = private GroupMemberName of string


///Others
/// 
/// 
type String50 = private String50 of string
    