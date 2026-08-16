namespace UserManagement.Domain.Entities;

public sealed class User
{
    private User() { }

    public User(string name, int age, string city, string state, string pincode)
    {
        Id = Guid.NewGuid();
        SetName(name);
        SetAge(age);
        SetCity(city);
        SetState(state);
        SetPincode(pincode);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int Age { get; private set; } = 0;
    public string City { get; private set; } = null!;
    public string State { get; private set; } = null!;
    public string Pincode { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        Name = name.Trim();
    }

    private void SetAge(int age)
    {
        if (age < 0 || age > 120)
            throw new ArgumentOutOfRangeException(
                nameof(age),
                age,
                "Age must be between 0 and 120.");

        Age = age;
    }

    private void SetCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.", nameof(city));

        City = city.Trim().ToLowerInvariant();
    }

    private void SetState(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State is required.", nameof(state));

        State = state.Trim().ToLowerInvariant();
    }

    private void SetPincode(string pincode)
    {
        if (string.IsNullOrWhiteSpace(pincode))
            throw new ArgumentException("City is required.", nameof(pincode));

        Pincode = pincode;
    }

    public void Update(
        string name,
        int age,
        string city,
        string state,
        string pincode)
    {
        Name = name;
        Age = age;
        City = city;
        State = state;
        Pincode = pincode;
    }
}
