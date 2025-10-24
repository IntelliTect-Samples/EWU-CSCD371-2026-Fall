namespace Logger;

public record Student(Guid Id, FullName fullName) : BasePerson(Id, fullName);
// This class does not declare IEntity directly;
// it inherits the implicit implementation from BasePerson (and BaseEntity).
