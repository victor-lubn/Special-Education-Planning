namespace SpecialEducationPlanning
.Domain.Enum
{
    public enum ErrorCode
    {

        //SystemErrors

        GenericSystemError,
        UndefinedUser,
        UnsuccessfulHttpCallError,


        //ControllerErrors 

        GenericControllerError,
        ArgumentErrorController,


        //RepositoryErrors 


        CommitRepositoryError,

        //DomainErrors 


        EntityNotFound,
        EntityAlreadyExist,


        //BusinessErrors

        GenericBusinessError,
        ArgumentErrorBusiness,
        NoResults,
        DeserializationError,

        ActionNotAllowed,
        UndefinedAiep,
        UndefinedEducationer,
        InvalidPostCode,
        InvalidAddress,
        MaxTakeExceeded,

        // InputErrors
        NullOrWhitespace

    }
}


