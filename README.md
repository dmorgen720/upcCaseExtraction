# upcCaseExtraction
A console application to download cases from the UPC (Unified Patent Court) using the public API

https://www.unified-patent-court.org/en/registry/it-developers for API scpecification

This application downloads the publicly available cases in batches of 100, starting by the page number given as parameter into a LiteDB database.
