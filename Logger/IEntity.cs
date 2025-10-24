using System;

namespace Logger;
public interface IEntity
{

    //Implemented implicitly in EntityBase
    //making it directly accessible to all consumers of the entity
    //without needing to cast to IEntity
    Guid Id { get; init; }

    //Implemented as abstract in EntityBase
    //so that each derived entity can calculate it appropriately
    string Name { get; }

}
