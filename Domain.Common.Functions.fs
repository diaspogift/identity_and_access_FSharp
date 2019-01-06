module public IdentityAndAcccess.CommonDomainTypes.Functions

open System
open IdentityAndAcccess.CommonDomainTypes




type FieldNameCreationMessage = FieldNameCreationMessage of string 






module ConstrainedType =



    let createString fieldName ctor maxLen str = 
            
        if String.IsNullOrEmpty(str) then
            let msg = sprintf "%s must not be null or empty" fieldName 
            Error msg
        elif str.Length > maxLen then
            let msg = sprintf "%s must not be more than %i chars" fieldName maxLen 
            Error msg 
        else
            let ok = Ok (ctor str)
            ok





    let createStringControlledLength fieldName ctor minLen maxLen str = 
            
        if String.IsNullOrEmpty(str) then
            let msg = sprintf "%s must not be null or empty" fieldName 
            Error msg
        elif str.Length > maxLen then
            let msg = sprintf "%s must not be more than %i chars" fieldName maxLen 
            Error msg 
        elif str.Length < minLen then
            let msg = sprintf "%s must not be less than %i chars" fieldName minLen 
            Error msg 
        else
            let ok = Ok (ctor str)
            ok





    let createStringOption fieldName ctor maxLen str = 
        if String.IsNullOrEmpty(str) then
            Ok None
        elif str.Length > maxLen then
            let msg = sprintf "%s must not be more than %i chars" fieldName maxLen 
            Error msg 
        else
            Ok (ctor str |> Some)

       




    let createInt fieldName ctor minVal maxVal i = 
        if i < minVal then
            let msg = sprintf "%s: Must not be less than %i" fieldName minVal
            Error msg
        elif i > maxVal then
            let msg = sprintf "%s: Must not be greater than %i" fieldName maxVal
            Error msg
        else
            Ok (ctor i)




        
    let createDecimal fieldName ctor minVal maxVal i = 
        if i < minVal then
            let msg = sprintf "%s: Must not be less than %M" fieldName minVal
            Error msg
        elif i > maxVal then
            let msg = sprintf "%s: Must not be greater than %M" fieldName maxVal
            Error msg
        else
            Ok (ctor i)




    let createLike fieldName  ctor pattern str = 
        if String.IsNullOrEmpty(str) then
            let msg = sprintf "%s: Must not be null or empty" fieldName 
            Error msg
        elif System.Text.RegularExpressions.Regex.IsMatch(str,pattern) then
            Ok (ctor str)
        else
            let msg = sprintf "%s: '%s' must match the pattern '%s'" fieldName str pattern
            Error msg 



    let createDatetimeSpan (fieldName:string) ctor (startDate:DateTime) (endDate:DateTime) =

        if DateTime.Compare (startDate, endDate) <=  0 then

            Ok { Start = ctor startDate; End = ctor endDate}

        else

            let msg = sprintf "%s: Start date must be inferior to end date" fieldName

            Error msg  
           

        
        
        

        
         





module String50 =

    let value (String50 str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName String50 50 str

    let create' = create "50 chars string"

    let createOption fieldName str = 
        ConstrainedType.createStringOption fieldName String50 50 str 
    
    let createOption' = createOption "Optional String"



module RoleId =



    let value (RoleId str) = str

    let create fieldName str = 
            ConstrainedType.createString fieldName RoleId 36 str
    let create' = create   "RoleId : "





module  RoleName =



    let value (RoleName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName RoleName 50 str

    let create' = create  "RoleName : "






module RoleDescription =



    let value (RoleDescription str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName RoleDescription 1000 str

    let create' = create   "RoleDescription : "





module TenantId =


    let value (TenantId str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName TenantId 36 str

    let create' = create  "TenantId : "







module  TenantName =


    let value (TenantName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName TenantName 250 str

    let create' = create  "TenantName : "







module TenantDescription =


    let value (TenantDescription str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName TenantDescription 1000 str

    let create' = create   "TenantDescription : "








module UserId =

    let value (UserId str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName UserId 36 str

    let create' = create  "UserId :"







module UserDescriptorId =

    let value (UserDescriptorId str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName UserDescriptorId 36 str

    let create' = create  "UserId :"






module Username =

    let value (Username str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName Username 50 str

    let create' = create  "Username :"







module Password =

    let value (Password str) = str

    let create fieldName str = 
        ConstrainedType.createStringControlledLength fieldName Password 6 50 str

    let create' = create  "Username :"






module FirstName =

    let value (FirstName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName FirstName 50 str

    let create' = create  "FirstName :"
        





module MiddleName =

    let value (MiddleName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName MiddleName 50 str

    let create' = create  "MiddleName :"







module LastName =

    let value (LastName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName LastName 100 str

    let create' = create  "LastName :"





module RegistrationInvitationId =

    let value (RegistrationInvitationId str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName RegistrationInvitationId 36 str

    let create' = create  "RegistrationInvitationId :"






module RegistrationInvitationDescription =

    let value (RegistrationInvitationDescription str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName RegistrationInvitationDescription 1000 str

    let create' = create  "RegistrationInvitationDescription :"






module GroupId =

    let value (GroupId str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName GroupId 36 str

    let create' = create  "GroupId :"





module GroupName =

    let value (GroupName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName GroupName 250 str

    let create' = create  "GroupName :"






module GroupDescription =

    let value (GroupDescription str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName GroupDescription 1000 str

    let create' = create  "GroupDescription : "






module GroupMemberId =

    let value (GroupMemberId str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName GroupMemberId 36 str

    let create' = create  "GroupMemberId : "




        


module GroupMemberName =

    let value (GroupMemberName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName GroupMemberName 150 str
    
    let create' = create  "GroupMemberName : "







module EmailAddress =

    let value (EmailAddress str) = str

    let create fieldName str = 
        let pattern = ".+@.+" 
        ConstrainedType.createLike fieldName EmailAddress pattern str

    let create' = create  "EmailAddress : "






module PostalAddress =

    let value (PostalAddress str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName PostalAddress 1000 str
    
    let create' = create  "PostalAddress : "






module Telephone =

    let value (Telephone str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName Telephone 50 str

    let create' = create  "Telephone : "






module DateTimeWrapped =

    let value (DateTimeWrapped dateTime) = dateTime

 




module DateTimeSpan =

    let create fieldName startDate endDate = 
        ConstrainedType.createDatetimeSpan fieldName DateTimeWrapped startDate endDate

    let create' = create  "Datetime Span : "






[<AutoOpen>]
module ResultComputationExpression =

    type ResultBuilder() =
        member __.Return(x) = Ok x
        member __.Bind(x, f) = Result.bind f x
    
        member __.ReturnFrom(x) = x
        member this.Zero() = this.Return ()

        member __.Delay(f) = f
        member __.Run(f) = f()

        member this.While(guard, body) =
            if not (guard()) 
            then this.Zero() 
            else this.Bind( body(), fun () -> 
                this.While(guard, body))  

        member this.TryWith(body, handler) =
            try this.ReturnFrom(body())
            with e -> handler e

        member this.TryFinally(body, compensation) =
            try this.ReturnFrom(body())
            finally compensation() 

        member this.Using(disposable:#System.IDisposable, body) =
            let body' = fun () -> body disposable
            this.TryFinally(body', fun () -> 
                match disposable with 
                    | null -> () 
                    | disp -> disp.Dispose())

        member this.For(sequence:seq<_>, body) =
            this.Using(sequence.GetEnumerator(),fun enum -> 
                this.While(enum.MoveNext, 
                    this.Delay(fun () -> body enum.Current)))

        member this.Combine (a,b) = 
            this.Bind(a, fun () -> b())

    let result = new ResultBuilder()

 

