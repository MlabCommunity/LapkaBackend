namespace LapkaBackend.Application.Tests.Helper;

public class AnimalMockBuilder
{
    private readonly Animal _animal = new ProxyGenerator().CreateClassProxy<Animal>();
    
    
    public AnimalMockBuilder WithName(string name)
    {
        _animal.Name = name;
        return this;
    }
    
    public AnimalMockBuilder WithSpecies(string species)
    {
        _animal.Species = species;
        return this;
    }

    public AnimalMockBuilder WithGender(string gender)
    {
        _animal.Gender = gender;
        return this;
    }
    
    public AnimalMockBuilder WithWeight(decimal weight)
    {
        _animal.Weight = weight;
        return this;
    }
    
    public AnimalMockBuilder WithDescription(string description)
    {
        _animal.Description = description;
        return this;
    }
    
    public AnimalMockBuilder WithCreatedAt(DateTime createdAt)
    {
        _animal.CreatedAt = createdAt;
        return this;
    }
    
    public AnimalMockBuilder WithIsSterilized(bool isSterilized)
    {
        _animal.IsSterilized = isSterilized;
        return this;
    }
    
    public AnimalMockBuilder WithIsVisible(bool isVisible)
    {
        _animal.IsVisible = isVisible;
        return this;
    }
    
    public AnimalMockBuilder WithMonths(int months)
    {
        _animal.Months = months;
        return this;
    }
    
    public AnimalMockBuilder WithIsArchival(bool isArchival)
    {
        _animal.IsArchival = isArchival;
        return this;
    }
    
    public Animal Build()
    {
        return _animal;
    }
    
}