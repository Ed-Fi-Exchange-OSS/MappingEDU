# User Guide - The Basics

This User Guide documentation contains conceptual and how-to information
for working with MappingEDU.

## Before You Begin

You should check out the [Getting Started](../getting-started)
documentation for high-level information about what MappingEDU does (and
does not do) and how to access the system.

## Data Standard Basics

When you create a data mapping in MappingEDU, you map one Data Standard
to another Data Standard. Data Standards are essentially like a data
dictionary: they contain entities, element names, element annotations,
and often contain type information (e.g., string, int, decimal) and
other properties (e.g., string length). The "mappings" are an expression
of the business logic that transforms elements in one standard to
another.

MappingEDU Data Standards can also contain enumerations and the value
lists for enumerations. This allows you to express mappings between
value-list-type items.

MappingEDU comes pre-loaded with a few common Data Standard definitions
such as various Ed-Fi data standards and CEDS versions. However, you are
not limited to the pre-loaded standards. You can enter a data standard
directly online, which is useful but tedious – so MappingEDU also allows
you to upload Data Standard definitions using a predefined Excel file
template.

See the [Data Standards](Data_Standards.md) section of this
documentation for more detail.

## Data Mapping Project Basics

To map one Data Standard to another, you set up a Data Mapping Project.
The Data Mapping Project allows you to pick a "Source" Data Standard,
that is, the standard you're mapping "from" and a "Target" Data Standard
which is the standard you're mapping "to."

See the [Mapping Projects](Mapping_Projects.md) section of
this documentation for more detail.

## Business Logic Basics

The Business Logic field is where the magic happens: this is where the
mapping between one standard and another is expressed. The mapping can
be as simple as a 1-to-1 field mapping, or a complex calculation. The
Business Logic field can identify elements from the target standard
which provides some structure – but the field also allows for free-form
text entry – so you can explain the mapping details using any
combination of elements and text that is most meaningful to you.

Not every field in a standard will have an appropriate mapping.
MappingEDU allows you to mark fields as "for Omission" (meaning that
there is no match in the Target standard), and "for Extension" (meaning
that the Target standard should be extended/expanded to convey the
information from the Source standard).

See the [Business Logic](Business_Logic.md) section of this
documentation for more detail.

## Reporting & Output Basics

Most information in MappingEDU can be exported or otherwise output.

The primary output of the system is the Mapping Review Report which
shows the complete business logic that maps the Source and Target Data
Standards, along with any notes, mapping statuses, and so forth. This
report can be output at any time during the life cycle of a project, so
you can give stakeholders a view into the status of your mapping
project.

See the [Mapping Review Report](Mapping_Review_Report.md)
section of this documentation for more detail.

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
