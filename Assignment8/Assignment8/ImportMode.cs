namespace Assignment8;

public enum ImportMode
{
    FailFast, //default --> stop importing on first invalid record
    SkipInvalid  //skip invalid records and continue importing
}
