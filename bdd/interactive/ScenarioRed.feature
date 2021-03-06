Feature: Refunded or replaced items should be returned to stock

Scenario: Refunded items should be returned to stock
    Given a customer buys a black jumper
    And I have 2 black jumpers left in stock
    When he returns the jumper for a refund
    Then I should have 4 black jumpers in stock