
// Typing for the LAB library, version 

/*
    Interface for the AJAX setting that will configure the AJAX request
*/
interface LabStatic {
    script(name: string): LabStatic;
    wait(): LabStatic;
    wait(any): LabStatic;
}

declare var $LAB: LabStatic;
