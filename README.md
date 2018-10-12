# Treasury Cash Flow Forecasting 
 Accurate cash forecasting is the lifeblood of any treasury department. All financial decisions, from near-term liquidity to strategic planning, are based
 on forecasts. However, treasurers continue to struggle creating timely and relevant forecast. 

 While creating a model template and the infrastrcture required to support a time series analysis, we can provide a solid starting point for our financial partners
 to better utilize Azure services while serving their customers.


## BUSINESS PROBLEM TO BE SOLVED
More than 70 percent of treasury executives are involved in cash position reporting and forecasting – more than any other activity – and it takes up half of their time1.

Long term (6+ months) is not nearly as difficult (usually within 4% accuracy) as a short term (1 day) forecast (nearly useless at this point). The key issues are that they cannot get data fast enough and members are making changes to the data too quickly.


## OBJECTIVE: Build a reusable infrastructure for deploying machine learning models. 
This will consist of two parts: walk then run. 

The model may take weeks or months to perfect, and will be unique to each customer, therefore our time is best spent scaffolding the infrastructure, which can be reused regardless of the model. 

### PART ONE
This is useful for data scientists who aren’t familiar with Azure and simply want to adopt our tools to build their models.

Initially I propose a bare-bones model which makes use of Microsoft’s forecasting library and documenting precisely how it can be used. This allows for buy-in from data scientists, engineers, and business side of the customer, due to its simplicity.

I have an example of it something similar here,  which includes a model created with Scikit-Learn, Flask for a web server, and Docker to containerize the environment. When I pitched this to PNC, all three of their teams instantly had light bulbs go off and were on the same page, which made me realize the value in this.

### PART TWO
When the customer is comfortable with the tools above, they can move to a more technical and in-depth solution, which makes use of additional Azure functionality.

This includes: 
ML model management, Azure Functions, Blob storage, and AAD.

This GitHub repository illustrates the complexity, but also the benefits of ML model management. Ignore the first half, which largely covers the soon-to-be-deprecated ML Studio. 
The value is largely in the environment preparation steps, which walks users through scaffolding an ML model management account. 
