<?php

namespace endpoint\items;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\database\NumericHelper;
use framework\entities\items\ItemsService;
use framework\entities\items\Item;
use framework\entities\items_link\ItemLink;
use framework\entities\items_link\ItemsLinkService;
use stdClass;

class InsertItems extends BaseEndpointBuilder
{
    private ItemsService $_itemsService;
    private ItemsLinkService $_itemsLinkService;

    public function __construct()
    {
        $this->_itemsService = new ItemsService();
        $this->_itemsLinkService = new ItemsLinkService();
        parent::__construct();
    }
    public function defaultParams()
    {
        return [
            'item' => null,
            'item_link' => null,
            'method' => "InsertOrUpdateItem",
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));
        if ($this->getParam('method') == "InsertOrUpdateItem") {
            $itemModel = (object) $this->getParam('item');
            $itemLinkModel = (object) $this->getParam('item_link');
            $result = new stdClass();
            if ($itemModel == null || $itemLinkModel == null) {
                $result->statusInsert = false;
                $result->statusType = 1;
                return $result;
            }
            if ($itemModel->id == null || $itemModel->id == "") {
                $item = new Item(
                    null,
                    $itemModel->label,
                    $itemModel->image,
                    NumericHelper::toIntOrNull($itemModel->category),
                    NumericHelper::toIntOrNull($itemModel->brand),
                    NumericHelper::toIntOrNull($itemModel->package),
                    NumericHelper::toFloatOrNull($itemModel->units),
                    NumericHelper::toFloatOrNull($itemModel->term),
                    $itemModel->barcodes,
                    $itemModel->consist,
                    NumericHelper::toFloatOrNull($itemModel->calorie),
                    NumericHelper::toFloatOrNull($itemModel->carbonhydrates),
                    NumericHelper::toFloatOrNull($itemModel->fat),
                    NumericHelper::toFloatOrNull($itemModel->proteins),
                    $itemModel->additional
                );
                $this->_itemsService->insertItemToDB($item);
                $item = $this->_itemsService->getLastInsertedItem();
                $statusInsert = $this->_itemsService->getItemFromDB($item->id)->label == $itemModel->label;
                if ($statusInsert) {
                    $itemLink = new ItemLink(
                        null,
                        $item->id,
                        NumericHelper::toIntOrNull($itemLinkModel->shopid),
                        NumericHelper::toIntOrNull($itemLinkModel->inshopid),
                        NumericHelper::toFloatOrNull($itemLinkModel->pricefactor)
                    );
                    $this->_itemsLinkService->insertItemToDB($itemLink);
                    $itemLink = $this->_itemsLinkService->getLastInsertedItem();
                    $statusInsert = $statusInsert && $this->_itemsLinkService->getItemFromDB($itemLink->id)->itemid == $item->id;
                }
                $result->statusType = 1;
                $result->statusInsert = $statusInsert;
                $result->item = $item;
            } else {
                $item = new Item(
                    NumericHelper::toInt($itemModel->id),
                    $itemModel->label,
                    $itemModel->image,
                    NumericHelper::toIntOrNull($itemModel->category),
                    NumericHelper::toIntOrNull($itemModel->brand),
                    NumericHelper::toIntOrNull($itemModel->package),
                    NumericHelper::toFloatOrNull($itemModel->units),
                    NumericHelper::toFloatOrNull($itemModel->term),
                    $itemModel->barcodes,
                    $itemModel->consist,
                    NumericHelper::toFloatOrNull($itemModel->calorie),
                    NumericHelper::toFloatOrNull($itemModel->carbonhydrates),
                    NumericHelper::toFloatOrNull($itemModel->fat),
                    NumericHelper::toFloatOrNull($itemModel->proteins),
                    $itemModel->additional
                );
                $this->_itemsService->updateItemInDB($item);
                $statusUpdate = $this->_itemsService->getItemFromDB($item->id)->label == $item->label;
                if ($statusUpdate) {
                    $itemLinks = $this->_itemsLinkService->getItemsFromDB([
                        "itemid" => [$itemModel->id],
                        "shopid" => [$itemLinkModel->shopid]
                    ]);
                    if (count($itemLinks) <= 0) {
                        $itemLink = new ItemLink(
                            null,
                            $item->id,
                            NumericHelper::toIntOrNull($itemLinkModel->shopid),
                            NumericHelper::toIntOrNull($itemLinkModel->inshopid),
                            NumericHelper::toFloatOrNull($itemLinkModel->pricefactor)
                        );
                        $this->_itemsLinkService->insertItemToDB($itemLink);
                        $itemLink = $this->_itemsLinkService->getLastInsertedItem();
                        $statusUpdate = $statusUpdate && $this->_itemsLinkService->getItemFromDB($itemLink->id)->itemid == $item->id;
                    }
                }
                $result->statusType = 2;
                $result->statusUpdate = $statusUpdate;
            }

            return $result;
        } elseif ($this->getParam('method') == "LinkItem") {
            $itemModel = (object) $this->getParam('item');
            $itemLinkModel = (object) $this->getParam('item_link');
            $result = new stdClass();
            $statusLink = null;

            $itemLinks = $this->_itemsLinkService->getItemsFromDB([
                "itemid" => [$itemModel->id],
                "shopid" => [$itemLinkModel->shopid]
            ]);
            if (count($itemLinks) <= 0) {
                $itemLink = new ItemLink(
                    null,
                    $itemModel->id,
                    NumericHelper::toIntOrNull($itemLinkModel->shopid),
                    NumericHelper::toIntOrNull($itemLinkModel->inshopid),
                    NumericHelper::toFloatOrNull($itemLinkModel->pricefactor)
                );
                $this->_itemsLinkService->insertItemToDB($itemLink);
                $itemLink = $this->_itemsLinkService->getLastInsertedItem();
                $statusLink = $this->_itemsLinkService->getItemFromDB($itemLink->id)->itemid == $itemModel->id;
            } else {
                $statusLink = false;
            }
            $result->statusType = 3;
            $result->statusLink = $statusLink;
            return $result;
        }
    }
}
