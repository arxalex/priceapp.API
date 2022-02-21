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
                '<script src="/system/scripts/admin/packageInsert.js"></script>'
            ],
            'links' => [
                '<link href="/system/style/admin/items.css" rel="stylesheet">'
            ]
        ];
    }
    public function defaultParams()
    {
        return [];
    }
    public function build()
    {

        return '<items></items>';
    }
}
