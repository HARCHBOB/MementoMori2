export class TagValidator {
    private tagList: string[];
    private tags: string;

    constructor() {
    }

    // Method to set and split tags
    public setTags(list: string) {
        this.tags = list;
        this.tagList = this.tags.split(';').map(tag => tag.trim());
    }

    // Method to validate tags and return an error message
    public returnError(): string {
        let commaNeeded = false;
        let returnValue = "Error: ";

        // Rule 1: Check if tag list is empty
        if (this.tagList.length === 0) {
            returnValue += "Tag list must not be empty";
            commaNeeded = true;
        }

        let emptyTagsCount = 0;
        let invalidStartCount = 0;

        // Check for empty or invalid tags
        for (const tag of this.tagList) {
            if (!tag) {
                emptyTagsCount++;
            }
            if (tag.length > 0 && !((tag[0] >= 'A' && tag[0] <= 'Z') || (tag[0] >= '0' && tag[0] <= '9'))) {
                invalidStartCount++;
            }
        }

        // Handle empty tag errors
        if (emptyTagsCount > 0) {
            if (commaNeeded) returnValue += ", ";
            returnValue += "Tags must be separated by ';'";
            commaNeeded = true;
        }

        // Handle invalid start character errors
        if (invalidStartCount > 0) {
            if (commaNeeded) returnValue += ", ";
            returnValue += "Tags must start with a capital letter or start with a number";
        }

        // If no errors, return empty string
        if (returnValue === "Error: ") {
            return "";
        }

        // Return the error message
        return returnValue + ";";
    }
}
