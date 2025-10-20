public class User
{
    public Guid UserID { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public bool ValidatePassword(string inputPassword)
    {
        //Would want to replace with hashing in future
        return Password == inputPassword;
    }

    public bool CheckConfigOwner(Guid configID)
    {
        // Placeholder logic
        return configID == UserID;
    }
}