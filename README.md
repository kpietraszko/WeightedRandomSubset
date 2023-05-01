# Losowy Podzbiór Ważonych Elementów
_english version below_

## Opis algorytmu

_algorytm zakłada, że możliwych priorytetów jest skończona liczba, wstępnie od P1 do P5_

1. Ułożyć elementy obok siebie na linii, gdzie waga (priorytet) elementu stanowi jego długość

![unordered](https://user-images.githubusercontent.com/17947254/235474753-5ee87d6d-c284-4c0f-9867-2f16f913fe30.svg)

2. Pogrupować i posortować elementy na tej linii po ich wadze (priorytecie)
3. Zapisać elementy w oddzielnych listach per waga (priorytet)

![grouped](https://user-images.githubusercontent.com/17947254/235474801-1dac13e2-d2e1-41f9-8937-0b10b129f424.svg)

4. N razy (gdzie N to liczba elementów jakie chcemy otrzymać):
  1. Wylosować punkt na powyższej linii (liczbę od 0 do sumy wag wszystkich elementów)
  2. Policzyć przedziały list z kroku 3 na linii (count listy razy jej waga)
  3. Znaleźć element który leży w tym punkcie (patrz poniżej)
  4. Skrócić linię (zakres losowania) o wagę wylosowanego elementu
  
#### Szczegóły kroku 4.iii
Na podstawie przedziałów z 4.ii określić w której liście jest wylosowany element/punkt.

Rozpatrywać tę listę jako krótszy odcinek na linii.  
Odległość wylosowanego punktu od początku tego odcinka, podzielić przez wagę (np. 1.2)  
Otrzymujemy indeks elementu na tej liście - to jest nasz wylosowany element.

# Random Subset Of Weighted Elements
## Algorithm description
_the algorithm assumes that there's a finite number of possible priorities - tentatively P1 to P5_

1. Lay out the elements on a line, the element's weight (priority) determines its length

![unordered](https://user-images.githubusercontent.com/17947254/235474753-5ee87d6d-c284-4c0f-9867-2f16f913fe30.svg)

2. Group and sort the elements on this line, by their weight (priority)
3. Store the elements in separate lists per weight (priority)

![grouped](https://user-images.githubusercontent.com/17947254/235474801-1dac13e2-d2e1-41f9-8937-0b10b129f424.svg)

4. N times (where N is the number of elements we wish to retrieve):
  1. Pick a random point on the line (number between 0 and all elements' weights' sum)
  2. Calculate the ranges of the lists from step 3 (list's count times its weight)
  3. Find out which element lies on this point (see below)
  4. Shorten the line (the randomisation range) by the weight of the picked element

  
#### Details of step 4.iii
Based on the ranges from step 4.ii determine on which list the element/point lies.
Consider this list as a line segment on the main line.  
Divide the distance of the points from this segment's start, by this list's weight (eg. 1.2)  
This gives us the index of the element on this list - that's our randomly picked element.
