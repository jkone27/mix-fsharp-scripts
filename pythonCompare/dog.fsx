type Dog(name,age) as this =
  member val Description = 
     $"{name} is {age} years old"
  member _.Speak(sound) =      
     $"{name}"
  member val Species = "canis familiaris"
