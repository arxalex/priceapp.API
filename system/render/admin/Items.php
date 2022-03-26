<?php

namespace render\admin;

use render\defaultBuild\BaseRenderBuilder;

class Items extends BaseRenderBuilder
{
    public function getExternalParams()
    {
        return [
            'header_title' => 'Items',
            'scripts' => [
                '<script src="/system/scripts/admin/items.js"></script>',
                '<script src="/system/scripts/admin/item.js"></script>',
                '<script src="/system/scripts/admin/itemsaver.js"></script>',
                '<script src="/system/scripts/admin/categoryInsert.js"></script>',
                '<script src="/system/scripts/admin/packageInsert.js"></script>',
                '<script src="/system/scripts/admin/brandInsert.js"></script>',
                '<script src="/system/scripts/admin/consistInsert.js"></script>',
                '<script src="/system/scripts/admin/countryInsert.js"></script>',
            ],
            'links' => [
                '<link href="/system/style/admin/items.css" rel="stylesheet">'
            ]
        ];
    }
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRedirect($this->getParam('cookie'), 9);
        return '<items></items>';
    }
}
