namespace Logger;

public record Employee(Guid Id, FullName fullName) : BasePerson(Id, fullName);
// This class does not declare IEntity directly;
// it inherits the implicit implementation from BasePerson (and BaseEntity).
