// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Constants Module 
//

var m = angular.module('app.constants', []); 
 

// ****************************************************************************
// System settings 
//

interface IOption {
    key: string,
    value: string,
}

interface ISystemSettings {
    apiBaseUri: string;
    appBaseUri: string;
    clientBaseUri: string;
    currentTheme: string;
    debugEnabled: boolean;
    directiveBaseUri: string;
    docsBaseUri: string;
    modalBaseUri: string;
    moduleBaseUri: string;
    themes: Array<string>;
    widgetsBaseUri: string;
    deliminator: string;
    version: string;
}

m.constant('settings', <ISystemSettings>{
    apiBaseUri: window['api_root'],
    appBaseUri: window['app_root'],
    clientBaseUri: window['client_root'],
    debugEnabled: true,
    docsBaseUri: window['api_root'] + '/docs/index',
    moduleBaseUri: window['client_root'] + '/app/modules',
    directiveBaseUri: window['client_root'] + '/app/directives',
    modalBaseUri: window['client_root'] + '/app/modals',
    deliminator: ', ',
    version: window['version']
});


// ****************************************************************************
// Enumerations
//

interface IEnumeration {
    Id: number,
    DisplayText: string,
}

interface IEnumerations {
    AutoMappingProjectSuggestionType: Array<IEnumeration>,
    CompleteStatusType: Array<IEnumeration>,
    ElementListColumns: Array<IEnumeration>,
    ElementMappingColumns: Array<IEnumeration>,
    EnumerationMappingStatusReasonType: Array<IEnumeration>,
    EnumerationMappingStatusType: Array<IEnumeration>,
    EnumerationListColumns: Array<IEnumeration>,
    EnumerationMappingColumns: Array<IEnumeration>,
    GapElementMappingColumns: Array<IEnumeration>,
    GapEnumerationMappingColumns: Array<IEnumeration>,
    ItemChangeType: Array<IEnumeration>,
    ItemDataType: Array<IEnumeration>,
    ItemType: Array<IEnumeration>,
    MappingMethodType: Array<IEnumeration>,
    MappingMethodTypeInQueue: Array<IEnumeration>,
    MappingStatusReasonType: Array<IEnumeration>,
    MappingStatusType: Array<IEnumeration>,
    ProjectStatusType: Array<IEnumeration>,
    UserAccess: Array<IEnumeration>,
    SystemConstantType: Array<IEnumeration>,
    WorkflowStatusType: Array<IEnumeration>
}

m.constant('enumerations', <IEnumerations> {

    AutoMappingProjectSuggestionType: [
        { Id: 1, DisplayText: 'Transitive' },
        { Id: 2, DisplayText: 'Previous Source Version to Target' },
        { Id: 3, DisplayText: 'Source to Previous Target Version' },
        { Id: 4, DisplayText: 'Target is Previous Version' },
        { Id: 5, DisplayText: 'Target is Next Version' }
    ],
    CompleteStatusType: [
        { Id: 1, DisplayText: 'Incomplete' },
        { Id: 2, DisplayText: 'Complete' },
        { Id: null, DisplayText: '' }
    ],
    ElementListColumns: [
        { Id: 1, DisplayText: 'Element Group' },
        { Id: 2, DisplayText: 'Entity' },
        { Id: 3, DisplayText: 'Element' },
        { Id: 4, DisplayText: 'Data Type' },
        { Id: 5, DisplayText: 'Field Length' },
        { Id: 6, DisplayText: 'Destination' },
        { Id: 7, DisplayText: 'Notes' },
        { Id: 8, DisplayText: 'Notes To' },
    ],
    ElementMappingColumns: [
        { Id: 1, DisplayText: 'System' },
        { Id: 2, DisplayText: 'Element Group' },
        { Id: 3, DisplayText: 'Entity' },
        { Id: 4, DisplayText: 'Element' },
        { Id: 5, DisplayText: 'Element Mapping Method' },
        { Id: 6, DisplayText: 'Element Workflow Status' },
        { Id: 7, DisplayText: 'Notes' },
        { Id: 8, DisplayText: 'Notes To' },
        { Id: 9, DisplayText: 'Omission Reason' },
        { Id: 10, DisplayText: 'Business Logic' },
        { Id: 11, DisplayText: 'Destination Version' },
        { Id: 12, DisplayText: 'Destination Element' },
        { Id: 13, DisplayText: 'Destination Complete Element Name' },
        { Id: 14, DisplayText: 'Created By' },
        { Id: 15, DisplayText: 'Creation Date' },
        { Id: 16, DisplayText: 'Updated By' },
        { Id: 17, DisplayText: 'Update Date' }
    ],
    EnumerationMappingStatusReasonType: [
        { Id: null, DisplayText: '' },
        { Id: 1, DisplayText: 'Aligns with Core Non-Type Element' },
        { Id: 2, DisplayText: 'Boolean' },
        { Id: 3, DisplayText: 'Derived Value' },
        { Id: 4, DisplayText: 'n/a in System' }
    ],
    EnumerationMappingStatusType: [
        { Id: null, DisplayText: '' },
        { Id: 1, DisplayText: 'Accepted: Core Type/DescriptorMap' },
        { Id: 2, DisplayText: 'Approved Core Type/DescriptorMap Extension' },
        { Id: 3, DisplayText: 'Approved Enumeration' },
        { Id: 4, DisplayText: 'Approved extension. Descriptor Enumeration' },
        { Id: 5, DisplayText: 'Ignored' },
        { Id: 6, DisplayText: 'Mapped: Core Type/DescriptorMap' },
        { Id: 7, DisplayText: 'Maps to Descriptor' },
        { Id: 8, DisplayText: 'Not Implementing As Enumeration' },
        { Id: 9, DisplayText: 'Omitted' },
        { Id: 10, DisplayText: 'Proposed Core Type/DescriptorMap Extension' },
        { Id: 11, DisplayText: 'Proposed Enumeration' },
        { Id: 12, DisplayText: 'Proposed extension. Descriptor Enumeration' }
    ],
    EnumerationListColumns: [
        { Id: 1, DisplayText: 'Element' },
        { Id: 2, DisplayText: 'Enumeration' },
        { Id: 3, DisplayText: 'Short Description' },
        { Id: 4, DisplayText: 'Description' },
        { Id: 5, DisplayText: 'Notes' },
        { Id: 6, DisplayText: 'Notes To' }
    ],
    EnumerationMappingColumns: [
        { Id: 1, DisplayText: 'Element Group' },
        { Id: 2, DisplayText: 'Entity' },
        { Id: 3, DisplayText: 'Element' },
        { Id: 4, DisplayText: 'Notes' },
        { Id: 5, DisplayText: 'Notes To' },
        { Id: 6, DisplayText: 'Enumeration' },
        { Id: 7, DisplayText: 'Short Description' },
        { Id: 8, DisplayText: 'Description' },
        { Id: 9, DisplayText: 'Mapping Status' },
        { Id: 10, DisplayText: 'Mapping Status Reason' },
        { Id: 11, DisplayText: 'Destination Enumeration' },
        { Id: 12, DisplayText: 'Destination Element' },
        { Id: 13, DisplayText: 'Destination Complete Element Name' },
        { Id: 14, DisplayText: 'Creation Date' },
        { Id: 15, DisplayText: 'Update Date' }
    ],
    GapElementMappingColumns: [
        { Id: 1, DisplayText: 'System' },
        { Id: 2, DisplayText: 'Element Group' },
        { Id: 3, DisplayText: 'Entity' },
        { Id: 4, DisplayText: 'Element' },
        { Id: 5, DisplayText: 'Workflow Status' },
        { Id: 6, DisplayText: 'Business Logic' },
        { Id: 7, DisplayText: 'Source Version' },
        { Id: 8, DisplayText: 'Source Element' },
        { Id: 9, DisplayText: 'Source Complete Element Name' },
        { Id: 10, DisplayText: 'Created By' },
        { Id: 11, DisplayText: 'Creation Date' },
        { Id: 12, DisplayText: 'Updated By' },
        { Id: 13, DisplayText: 'Update Date' }
    ],
    GapEnumerationMappingColumns: [
        { Id: 1, DisplayText: 'Element Group' },
        { Id: 2, DisplayText: 'Entity' },
        { Id: 3, DisplayText: 'Element' },
        { Id: 4, DisplayText: 'Enumeration' },
        { Id: 5, DisplayText: 'Short Description' },
        { Id: 6, DisplayText: 'Description' },
        { Id: 7, DisplayText: 'Mapping Status' },
        { Id: 8, DisplayText: 'Mapping Status Reason' },
        { Id: 9, DisplayText: 'Source Enumeration' },
        { Id: 10, DisplayText: 'Source Element' },
        { Id: 11, DisplayText: 'Source Complete Element Name' },
        { Id: 12, DisplayText: 'Creation Date' },
        { Id: 13, DisplayText: 'Update Date' }
    ],
    ItemChangeType: [
        { Id: 0, DisplayText: '' },
        { Id: 1, DisplayText: 'Added Domain' },
        { Id: 2, DisplayText: 'Added Entity' },
        { Id: 3, DisplayText: 'Added Element' },
        { Id: 4, DisplayText: 'Changed Entity' },
        { Id: 5, DisplayText: 'Changed Element' },
        { Id: 6, DisplayText: 'Deleted Entity' },
        { Id: 7, DisplayText: 'Deleted Element' }
    ],
    ItemDataType: [
        { Id: null, DisplayText: '' },
        { Id: 1, DisplayText: 'Boolean' },
        { Id: 2, DisplayText: 'Byte' },
        { Id: 3, DisplayText: 'Char' },
        { Id: 4, DisplayText: 'Currency' },
        { Id: 5, DisplayText: 'Date' },
        { Id: 6, DisplayText: 'DateTime' },
        { Id: 7, DisplayText: 'Decimal' },
        { Id: 8, DisplayText: 'Double' },
        { Id: 9, DisplayText: 'Duration' },
        { Id: 10, DisplayText: 'Integer' },
        { Id: 11, DisplayText: 'Long' },
        { Id: 12, DisplayText: 'Short' },
        { Id: 13, DisplayText: 'String' },
        { Id: 14, DisplayText: 'Time' },
        { Id: 15, DisplayText: 'UniqueId' },
        { Id: 16, DisplayText: 'Year' },
        { Id: 17, DisplayText: 'Enumeration' }
    ],
    ItemType: [
        { Id: 1, DisplayText: 'Domain' },
        { Id: 2, DisplayText: 'Entity' },
        { Id: 3, DisplayText: 'SubEntity' },
        { Id: 4, DisplayText: 'Element' },
        { Id: 5, DisplayText: 'Enumeration' },
        { Id: 0, DisplayText: 'Unknown' }
    ],
    MappingMethodType: [
        { Id: 1, DisplayText: 'Enter Mapping Business Logic' },
        { Id: 3, DisplayText: 'Mark for Extension' },
        { Id: 4, DisplayText: 'Mark for Omission' },
        { Id: 0, DisplayText: 'Unknown' }
    ],
    MappingMethodTypeInQueue: [
        { Id: 1, DisplayText: 'Mapping Business Logic' },
        { Id: 3, DisplayText: 'Mark for Extension' },
        { Id: 4, DisplayText: 'Mark for Omission' },
        { Id: 0, DisplayText: 'Unknown' }
    ],
    MappingStatusReasonType: [
        { Id: null, DisplayText: '' },
        { Id: 1, DisplayText: 'Adult Education' },
        { Id: 2, DisplayText: 'Application/Evaluation/Eligibility' },
        { Id: 3, DisplayText: 'Assessment Form Design' },
        { Id: 4, DisplayText: 'Assessment Runtime' },
        { Id: 5, DisplayText: 'Change/Transaction Info' },
        { Id: 6, DisplayText: 'Data Warehouse Requirement' },
        { Id: 7, DisplayText: 'Definition/Purpose Unclear' },
        { Id: 8, DisplayText: 'EdFacts' },
        { Id: 9, DisplayText: 'Health Information' },
        { Id: 10, DisplayText: 'Mapped in Non-Snapshot Version' },
        { Id: 11, DisplayText: 'No current K12 performance use case' },
        { Id: 12, DisplayText: 'No known data source' },
        { Id: 13, DisplayText: 'Pending Use Case' },
        { Id: 14, DisplayText: 'Per Client Feedback' },
        { Id: 15, DisplayText: 'Slowly Changing Dimension' },
        { Id: 16, DisplayText: 'Student Performance' },
        { Id: 17, DisplayText: 'TSDL' }
    ],
    MappingStatusType: [
        { Id: null, DisplayText: '' },
        { Id: 1, DisplayText: 'Approved for Inclusion' },
        { Id: 2, DisplayText: 'Approved for Omission' },
        { Id: 3, DisplayText: 'Excluding Snapshot' },
        { Id: 4, DisplayText: 'Need Further Review' },
        { Id: 5, DisplayText: 'On Hold' },
        { Id: 6, DisplayText: 'Out of Scope' },
        { Id: 7, DisplayText: 'Proposed for Inclusion' },
        { Id: 8, DisplayText: 'Proposed for Omission' },
        { Id: 9, DisplayText: 'Approved DWH Extension' },
        { Id: 10, DisplayText: 'Proposed DWH Extension' }
    ],
    ProjectStatusType: [
        { Id: 1, DisplayText: 'Active' },
        { Id: 2, DisplayText: 'Closed' },
        { Id: 0, DisplayText: 'Unknown' }
    ],
    UserAccess: [
        { Id: 0, DisplayText: 'Guest'},
        { Id: 1, DisplayText: 'View' },
        { Id: 2, DisplayText: 'Edit' },
        { Id: 99, DisplayText: 'Owner' }
    ],
    SystemConstantType: [
        { Id: 0, DisplayText: 'Unknown' },
        { Id: 1, DisplayText: 'Text' },
        { Id: 2, DisplayText: 'ComplexText' },
        { Id: 3, DisplayText: 'Boolean' }
    ],
    WorkflowStatusType: [
        { Id: 0, DisplayText: 'Unmapped' },
        { Id: 1, DisplayText: 'Incomplete' },
        { Id: 2, DisplayText: 'Complete' },
        { Id: 3, DisplayText: 'Reviewed' },
        { Id: 4, DisplayText: 'Approved' }
    ]
})