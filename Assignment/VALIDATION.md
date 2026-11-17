# Assignment 8 - CSV Validation Rules

## Overview
The PeopleImporter reads CSV records and converts valid records into Person and Address objects.

## Validation Rules

1. Field Count
   Each record must contain exactly eight fields.
   Records with missing or extra fields are rejected.

2. Valid ID
   Each ID must be a positive integer.
   Zero, negative, and nonnumeric IDs are rejected.

3. Required Fields
   FirstName, LastName, Email, StreetAddress, City, State, and Zip cannot be empty or whitespace.

4. Duplicate IDs
   Each accepted person must have a unique ID.
   Duplicate IDs are rejected.

## Import Modes

FailFast:
Stops at the first invalid record and throws an InvalidPersonRecordException containing the line number, original record, and underlying exception.

SkipInvalid:
Reports each invalid record through a callback, continues processing, and returns valid people.

## Resource Management

PeopleImporter owns its TextReader and implements IDisposable. 
The caller must dispose the importer preferably using a using statement.

The importer cannot be used after disposal.

## Extra Credit

### Custom Validation

PeopleImporter accepts an optional validation delegate that allows callers to define additional rules.

For example, a caller can require that a person's
state is "WA".

- A result of true accepts the record.
- A result of false follows the selected import mode.
- FailFast throws InvalidPersonRecordException.
- SkipInvalid reports the rejected record and continues.
- Unexpected exceptions from the delegate propagate unchanged.

### Exception Filtering

The importer uses a when filter to handle only FormatException and OverflowException during parsing.

Unexpected exceptions, including I/O errors and exceptions from callbacks or custom validation, are allowed to propagate without being swallowed.

### Extra Credit Tests

Six additional tests verify custom validation, both import modes, exception propagation, and reuse of IDs from rejected records.