/**
 * Fetch  contact by id
 * 
 * Related Web resource:
 *   1. Utility.Http.js
 *   
 * @param {string} contactId: Guid of contact record.
 * @param {function} callBack: Function.
 * @param {any} parametersToCallBack: Any extra parameters.
 */
function GetById(contactId, callBack, parametersToCallBack) {
    const enrollmentStatusUrl = `${organizationUrl}contacts?$select=*&$filter=contactid%20eq%20${contactId}`;

    get(enrollmentStatusUrl, callBack, parametersToCallBack);
}