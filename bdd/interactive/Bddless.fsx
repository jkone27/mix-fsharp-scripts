type Inventory = { Count: int }


let ``Given I have {X} black jumpers left in stock`` n continuation =
     { Count = n }

let ``Given a customer buys a black jumper`` inventory continuation =
     { inventory with Count = Count - 1 }

let ``When he returns the jumper for a refund`` inventory continuation =
     { inventory with Count = Count + 1 }

let ``I should have {X} black jumpers in stock`` n inventory continuation =
    inventory.Count == n
