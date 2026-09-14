CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE EXTENSION IF NOT EXISTS citext;

CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "GoogleId" character varying(255) NOT NULL,
    "Email" citext NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "Equalizations" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "RequestId" uuid NOT NULL,
    "SourceShoppingListId" uuid NOT NULL,
    "Code" character varying(32) NOT NULL,
    "ShoppingListName" character varying(150) NOT NULL,
    "CreatedByName" character varying(150) NOT NULL,
    "CreatedByEmail" character varying(320) NOT NULL,
    "BestChoiceTotal" numeric(18,2) NOT NULL,
    "BestCompleteSupplierName" character varying(200),
    "BestCompleteSupplierTotal" numeric(18,2),
    "EstimatedEconomy" numeric(18,2) NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Equalizations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Equalizations_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PurchaseOrders" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "SourceShoppingListId" uuid,
    "SupplierId" uuid,
    "SourceEqualizationId" uuid,
    "Code" character varying(32) NOT NULL,
    "ShoppingListName" character varying(150) NOT NULL,
    "SupplierName" character varying(200) NOT NULL,
    "BuyerName" character varying(150) NOT NULL,
    "BuyerEmail" character varying(320),
    "ExpectedDeliveryDate" date,
    "DeliveryAddress" character varying(500),
    "PaymentTerms" character varying(200),
    "Notes" character varying(1000),
    "Status" integer NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "UpdatedAtUtc" timestamp with time zone NOT NULL,
    "CompletedAtUtc" timestamp with time zone,
    "CancelledAtUtc" timestamp with time zone,
    CONSTRAINT "PK_PurchaseOrders" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PurchaseOrders_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "QuotationRequests" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "SourceShoppingListId" uuid,
    "Code" character varying(32) NOT NULL,
    "ShoppingListName" character varying(150) NOT NULL,
    "Description" character varying(500),
    "BuyerName" character varying(150) NOT NULL,
    "BuyerEmail" character varying(320) NOT NULL,
    "ResponseDeadline" date,
    "DeliveryAddress" character varying(500),
    "Instructions" character varying(2000),
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_QuotationRequests" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_QuotationRequests_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ShoppingLists" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Name" character varying(150) NOT NULL,
    "Description" character varying(500),
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ShoppingLists" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ShoppingLists_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Suppliers" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Name" citext NOT NULL,
    "Cnpj" character varying(14),
    "AddressStreet" character varying(200),
    "AddressCity" character varying(100),
    "AddressPostalCode" character varying(8),
    "ContactEmail" character varying(254),
    "ContactPhone" character varying(13),
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Suppliers" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Suppliers_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "EqualizationItems" (
    "Id" uuid NOT NULL,
    "SavedEqualizationId" uuid NOT NULL,
    "SourceShoppingItemId" uuid NOT NULL,
    "Position" integer NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Quantity" numeric(19,3) NOT NULL,
    "Unit" character varying(50) NOT NULL,
    CONSTRAINT "PK_EqualizationItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EqualizationItems_Equalizations_SavedEqualizationId" FOREIGN KEY ("SavedEqualizationId") REFERENCES "Equalizations" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PurchaseOrderItems" (
    "Id" uuid NOT NULL,
    "PurchaseOrderId" uuid NOT NULL,
    "SourceShoppingItemId" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Quantity" numeric(19,3) NOT NULL,
    "Unit" character varying(50) NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    CONSTRAINT "PK_PurchaseOrderItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId" FOREIGN KEY ("PurchaseOrderId") REFERENCES "PurchaseOrders" ("Id") ON DELETE CASCADE
);

CREATE TABLE "QuotationRequestItems" (
    "Id" uuid NOT NULL,
    "QuotationRequestId" uuid NOT NULL,
    "SourceShoppingItemId" uuid,
    "Position" integer NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Quantity" numeric(19,3) NOT NULL,
    "Unit" character varying(20) NOT NULL,
    CONSTRAINT "PK_QuotationRequestItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_QuotationRequestItems_QuotationRequests_QuotationRequestId" FOREIGN KEY ("QuotationRequestId") REFERENCES "QuotationRequests" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ShoppingItems" (
    "Id" uuid NOT NULL,
    "ShoppingListId" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Quantity" numeric(19,3) NOT NULL,
    "Unit" character varying(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ShoppingItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ShoppingItems_ShoppingLists_ShoppingListId" FOREIGN KEY ("ShoppingListId") REFERENCES "ShoppingLists" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ShoppingListSuppliers" (
    "ShoppingListId" uuid NOT NULL,
    "SupplierId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ShoppingListSuppliers" PRIMARY KEY ("ShoppingListId", "SupplierId"),
    CONSTRAINT "FK_ShoppingListSuppliers_ShoppingLists_ShoppingListId" FOREIGN KEY ("ShoppingListId") REFERENCES "ShoppingLists" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ShoppingListSuppliers_Suppliers_SupplierId" FOREIGN KEY ("SupplierId") REFERENCES "Suppliers" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "EqualizationQuotes" (
    "Id" uuid NOT NULL,
    "SavedEqualizationItemId" uuid NOT NULL,
    "SourceSupplierId" uuid NOT NULL,
    "SupplierName" character varying(200) NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    "IsLowest" boolean NOT NULL,
    CONSTRAINT "PK_EqualizationQuotes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EqualizationQuotes_EqualizationItems_SavedEqualizationItemId" FOREIGN KEY ("SavedEqualizationItemId") REFERENCES "EqualizationItems" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ItemQuotes" (
    "Id" uuid NOT NULL,
    "ShoppingItemId" uuid NOT NULL,
    "SupplierId" uuid NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ItemQuotes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ItemQuotes_ShoppingItems_ShoppingItemId" FOREIGN KEY ("ShoppingItemId") REFERENCES "ShoppingItems" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ItemQuotes_Suppliers_SupplierId" FOREIGN KEY ("SupplierId") REFERENCES "Suppliers" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_EqualizationItems_SavedEqualizationId" ON "EqualizationItems" ("SavedEqualizationId");

CREATE UNIQUE INDEX "IX_EqualizationItems_SavedEqualizationId_Position" ON "EqualizationItems" ("SavedEqualizationId", "Position");

CREATE INDEX "IX_EqualizationQuotes_SavedEqualizationItemId" ON "EqualizationQuotes" ("SavedEqualizationItemId");

CREATE UNIQUE INDEX "IX_EqualizationQuotes_SavedEqualizationItemId_SourceSupplierId" ON "EqualizationQuotes" ("SavedEqualizationItemId", "SourceSupplierId");

CREATE UNIQUE INDEX "IX_Equalizations_Code" ON "Equalizations" ("Code");

CREATE INDEX "IX_Equalizations_UserId_CreatedAtUtc" ON "Equalizations" ("UserId", "CreatedAtUtc");

CREATE UNIQUE INDEX "IX_Equalizations_UserId_RequestId" ON "Equalizations" ("UserId", "RequestId");

CREATE INDEX "IX_ItemQuotes_ShoppingItemId" ON "ItemQuotes" ("ShoppingItemId");

CREATE INDEX "IX_ItemQuotes_SupplierId" ON "ItemQuotes" ("SupplierId");

CREATE INDEX "IX_PurchaseOrderItems_PurchaseOrderId" ON "PurchaseOrderItems" ("PurchaseOrderId");

CREATE UNIQUE INDEX "IX_PurchaseOrders_Code" ON "PurchaseOrders" ("Code");

CREATE INDEX "IX_PurchaseOrders_SourceEqualizationId" ON "PurchaseOrders" ("SourceEqualizationId");

CREATE INDEX "IX_PurchaseOrders_UserId_CreatedAtUtc" ON "PurchaseOrders" ("UserId", "CreatedAtUtc");

CREATE UNIQUE INDEX "IX_PurchaseOrders_UserId_SourceShoppingListId_SupplierId" ON "PurchaseOrders" ("UserId", "SourceShoppingListId", "SupplierId") WHERE "Status" <> 3 AND "SourceShoppingListId" IS NOT NULL AND "SupplierId" IS NOT NULL;

CREATE INDEX "IX_QuotationRequestItems_QuotationRequestId" ON "QuotationRequestItems" ("QuotationRequestId");

CREATE UNIQUE INDEX "IX_QuotationRequests_Code" ON "QuotationRequests" ("Code");

CREATE INDEX "IX_QuotationRequests_UserId_CreatedAtUtc" ON "QuotationRequests" ("UserId", "CreatedAtUtc");

CREATE INDEX "IX_ShoppingItems_ShoppingListId" ON "ShoppingItems" ("ShoppingListId");

CREATE INDEX "IX_ShoppingLists_UserId" ON "ShoppingLists" ("UserId");

CREATE INDEX "IX_ShoppingListSuppliers_SupplierId" ON "ShoppingListSuppliers" ("SupplierId");

CREATE UNIQUE INDEX "IX_Suppliers_UserId_Cnpj" ON "Suppliers" ("UserId", "Cnpj") WHERE "Cnpj" IS NOT NULL;

CREATE UNIQUE INDEX "IX_Suppliers_UserId_Name" ON "Suppliers" ("UserId", "Name");

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

CREATE UNIQUE INDEX "IX_Users_GoogleId" ON "Users" ("GoogleId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260914011249_InitialPostgres', '8.0.11');

COMMIT;

