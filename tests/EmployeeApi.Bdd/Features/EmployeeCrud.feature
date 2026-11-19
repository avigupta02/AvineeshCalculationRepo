@AS-20 @employee-crud
Feature: Employee CRUD through the JSON-backed API
  As an administrator
  I want to manage employee records through the Web API
  So that employee data is persisted in the JSON file

  @AS-21 @create
  Scenario: Create and persist a valid employee
    Given the employee store is empty
    When I create employee "1001" with name "Ada Lovelace", email "ada@example.com", and department "Engineering"
    Then the response status is 201
    And the employee "1001" is persisted with name "Ada Lovelace", email "ada@example.com", and department "Engineering"

  @AS-21 @create @validation
  Scenario: Reject a duplicate employee ID
    Given an employee with ID "1001" already exists
    When I create employee "1001" with name "Grace Hopper", email "grace@example.com", and department "Engineering"
    Then the response status is 409
    And no duplicate employee is persisted

  @AS-21 @create @validation
  Scenario: Reject invalid employee data without changing the store
    Given the employee store contains one employee
    When I create an employee with a missing required field or invalid email
    Then the response is a validation error
    And the employee store is unchanged

  @AS-22 @read
  Scenario: Retrieve all employees
    Given the employee store contains employee records
    When I request GET "/employees"
    Then the response status is 200
    And the response contains each employee with ID, name, email, and department

  @AS-22 @read
  Scenario: Retrieve an existing employee
    Given an employee with ID "1001" exists
    When I request GET "/employees/1001"
    Then the response status is 200
    And the response contains employee "1001"

  @AS-22 @read @not-found
  Scenario: Return not found for an unknown employee
    Given no employee with ID "9999" exists
    When I request GET "/employees/9999"
    Then the response status is 404

  @AS-22 @read @empty
  Scenario: Retrieve an empty employee collection
    Given the employee store is empty
    When I request GET "/employees"
    Then the response status is 200
    And the response is an empty collection

  @AS-23 @update
  Scenario: Update and persist an existing employee
    Given an employee with ID "1001" exists
    When I update employee "1001" with name "Ada Byron", email "ada.byron@example.com", and department "Research"
    Then the response status is 204
    And a subsequent read of employee "1001" returns the updated values

  @AS-23 @update @not-found
  Scenario: Do not append an unknown employee during update
    Given no employee with ID "9999" exists
    When I update employee "9999" with valid values
    Then the response status is 404
    And no employee is appended

  @AS-23 @update @validation
  Scenario: Preserve an employee when update data is invalid
    Given an employee with ID "1001" exists
    When I update employee "1001" with invalid data
    Then the response is a validation error
    And the prior employee record is preserved

  @AS-24 @delete
  Scenario: Delete an existing employee and preserve remaining records
    Given employees with IDs "1001" and "1002" exist
    When I request DELETE "/employees/1001"
    Then the response status is 204
    And employee "1001" is absent from the collection
    And employee "1002" remains in the collection

  @AS-24 @delete @not-found
  Scenario: Return not found when deleting an unknown employee
    Given no employee with ID "9999" exists
    When I request DELETE "/employees/9999"
    Then the response status is 404
    And the employee store is unchanged
