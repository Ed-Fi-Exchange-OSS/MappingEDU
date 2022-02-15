# User Guide - Mapping Helper

The Mapping Helper is a feature that suggests possible mappings based on
element paths in the current and other available projects.

## How it Works

The Mapping Helper applies a few simple algorithms to identify a
potential match:

1. Identifies and suggests mappings between items with the same path and type
    between the two data standards (for this step it does not even need to query
    other projects).
2. Identifies and suggests mappings based on mappings from other projects
    following these rules:
    * The other project must be active.
    * The user that is creating the project must have access to the project.
    * The target and destination of the project must be exactly the same version
      as the project being created.

The Mapping Helper can provide a helpful starting point for mapping two
standards, but the user must supplement its capabilities with detailed
understanding of the semantics and structure of the standards being
mapped.

## User Guide Contents

Read more about how to use MappingEDU:

* [The Basics](The_Basics.md)
* [Data Standards](Data_Standards.md)
* [Mapping Projects](Mapping_Projects.md)
* [Business Logic](Business_Logic.md)
* [Matchmaker](Matchmaker.md)
* [Workflow](Workflow.md)
* [Mapping Review Report](Mapping_Review_Report.md)
* [Mapping Helper](Mapping_Helper.md)
