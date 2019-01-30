module IdentityAndAccess.DomainServiceInterfaceTypes.Implementation


open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes





///Password related services implementations

let encryptPasswordService (aPleinTextPassword:Password) = 
    

    try 

        let strPassword = aPleinTextPassword |> Password.value
        
        
        let encriptePassword = EncrytedPassword.create' strPassword

        encriptePassword

    with
        | :? System.FormatException as ex -> Error "error0"    
        | :? System.TypeInitializationException as ex -> Error "error1"    
        | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
        | Failure msg -> Error (msg + ": By ...Faillure msg..." )
        | _ -> Error "Unmatched error occurred" 






///User related service implementations

let authenticateUserService  
     (loadUserByUserIdAndTenantId:LoadUserByUserIdPasswordAndTenantId) //Database Dependency 
     (loadTenantById:LoadTenantById) //Database Dependency 
     (passwordEncryptionService:PasswordEncryptionService) //Encription Dependency 
     (userId:UserId) 
     (tenantId:TenantId) 
     (aPleinTextPassword:Password) 
     : Result<UserDescriptor,string> = 
    
    try 

          result {
               let! tenant = loadTenantById tenantId
               let unwrappedPassword = aPleinTextPassword |> Password.value 
               let! aPleinTextStrongPassword = unwrappedPassword |> StrongPassword.create'
               let! encrytedPassword = aPleinTextStrongPassword |> passwordEncryptionService
               let! userToAuthenticate =  loadUserByUserIdAndTenantId userId encrytedPassword tenant.TenantId 
           
               let! userDescriptor =
                    match userToAuthenticate.Enablement.EnablementStatus with
                    | EnablementStatus.Enabled ->
                        userToAuthenticate 
                        |> User.toUserDesriptor
                    | EnablementStatus.Disabled ->
                        let msg = sprintf "User enablement status is disabled"
                        Error msg  

               return userDescriptor
               } 
     with
     | :? System.FormatException as ex -> Error "error0"    
     | :? System.TypeInitializationException as ex -> Error "error1"    
     | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
     | Failure msg -> Error (msg + ": By ...Faillure msg..." )
     | _ -> Error "Unmatched error occurred" 
