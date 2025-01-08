# santander-hackernews
Repository for developer task based on hacker news api


1. The solution uses clean architecture structure for projects with layers, this is a bit overkill for the task, but I decided to follow such approach as this is usually ok for building microservices
- Api - thin layer of controllers and application setup
- Application - holds the business logic
- Domain - should contain the domain objects, for simplicity it contains Item.cs which should be only available in infrastructure layer and then mapped to domain entity for processing
- Infrastructure - all external dependencies for the solution, could be db access, external services etc.
2. There could be a couple of possible solutions to limit the number of concurrent requests to fetch the items
For single instance system:
- Rate limiter middleware - this could be setup on the controller level to prevent too many concurrent requests from the users
- Lock mechanizm - in proces lock that only allows x amount of concurrent requests against the external resource
- Decreasing the number of concurrent requests with processing in buckets - the strategy would be to divide the available pool of item ids into separate buckets, then each bucket would process items sequentially
For horizontally scaled system:
- Rate limiter on Api gateway - prevent multiple requests to the resource by having additional layer of API Gateway that would route requestes to application instances
- Distributed lock - for this purpose Redis could be used which allows to increment a value atomically, then the instances could check the current value and wait if it exceeds the given limit
- Queue - if async processing is allowed to satisfy the api clients, with more advanced usage we could introduce a queue that would decouple producers from consumers. 
  The producer would put a message on a queue for each customer request, so we could service thousands of requests at the same time.
  The queue would work like a buffer for requests to be processed.
  The consumer would pick up messages from the queue at a stable pace satisfying the requirement to not overload the external resource and perform any heavy computations.
  At the end the user might receive the result with for example push notifications.
  With this approach we would decouple producers from consumers, allow them to scale independently and assure load leveling.
- Caching - we could introduce a layer of caching for the particular items for example Redis, that would offload the external server.
  This would introduce a problem of stale data, but this might be mitigated with the update capabilities of hacker news api, which is stated in the docs.
  Not sure how this is working, if it's a list of recently changed items, that needs to be retrieved from the api, or some push notifications that the client api could subscribe to.
  If the clients of our API are ok with trading a bit of data consistency for performance, this might be an option worth exploring