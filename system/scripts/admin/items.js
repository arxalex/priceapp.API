Vue.component('Items', {
    template: `
        <div>
            <h1>Prepare items</h1>
            <p>List:
                <div v-for="shop in shops">
                    <span>{{ shop.id }}</span> - <span>{{ shop.label }}</span>
                </div>
            </p>
            <div class="input-group mb-3">
                <span>
                <select class="form-select" v-model="selectedShopId">
                    <option selected disabled>Chose shop</option>
                    <option v-for="shop in shops" v-bind:value="shop.id">{{ shop.label }}</option>
                </select>
                <select class="form-select" v-model="selectedCategoryId">
                    <option selected disabled>Chose category</option>
                    <option v-for="shopCategory in shopCategories" v-bind:value="shopCategory.id">{{ shopCategory.label }}</option>
                </select>
            </div>
            <div class="input-group mb-3">
                <button @click="get_items()" class="btn btn-primary mt-2 w-100">Get items</button>
            </div>
            <div class="col-sm-12">
                <item class="mb-3" 
                    v-for="itemModel in itemModels" 
                    :item="itemModel.item" 
                    :similarItems="itemModel.similarItems"
                    :labels="itemModel.labels"
                    :similarLabels="itemModel.similarLabels">
                </item>
            </div>
        </div>
    `,
    data() {
        return {
            shops: {
                silpo: {
                    label: 'silpo',
                    id: 1,
                },
                atb: {
                    label: 'atb',
                    id: 3,
                },
                auchan: {
                    label: 'auchan',
                    id: 2,
                }
            },
            selectedShopId: null,
            selectedCategoryId: null,
            itemModels: [],
            shopCategories: []
        }
    },
    methods: {
        get_categories: async function() {
            const categoriesUrl = "../be/categories/get_categories";
            var shop = this.selectedShopId;
            var data = {
                source: shop
            }
            var categories = await this.getItemsFromDb(categoriesUrl, data);
            this.shopCategories = categories;
        },
        get_items: async function () {
            const similarUrl = "../be/items/get_similar_items";
            const labelsUrl = "../be/items/get_labels";
            const getItemUrl = "../be/items/get_items";

            var shop = this.selectedShopId;
            var category = this.selectedCategoryId;

            var data = {
                source: shop,
                category: category
            };

            var items = await this.getItemsFromDb(getItemUrl, data);

            var itemLabels = [];
            items.forEach(element => {
                itemLabels.push(element.label);
            });
            var similarItems = await this.getItemsFromDb(similarUrl, {
                itemLabels: itemLabels
            });

            var labelsRequest = [];
            items.forEach(element => {
                labelsRequest.push({
                    categoryId: element.category,
                    brandId: element.brand,
                    packageId: element.package,
                    consistIds: element.consist,
                    countryId: element.additional.country
                });
            });
            var labels = await this.getItemsFromDb(labelsUrl, {
                labelIds: labelsRequest
            });

            var similarLabels = [];
            
            for(var i = 0; i < similarItems.length; i++){
                var similarLabelRequests = [];
                similarItems[i].forEach(element => {
                    similarLabelRequests.push({
                        categoryId: element.category,
                        brandId: element.brand,
                        packageId: element.package,
                        consistIds: element.consist,
                        countryId: element.additional.country
                    });
                });
                var similarLabel = await this.getItemsFromDb(labelsUrl, {
                    labelIds: similarLabelRequests
                });
                similarLabels.push(similarLabel);
            }

            for(var i = 0; i < items.length; i++){
                this.itemModels.push({
                    item: items[i],
                    similarItems: similarItems[i],
                    labels: labels[i],
                    similarLabels: similarLabels[i]
                });
            }
            console.log(this.itemModels);
        },
        getItemsFromDb: function (url, data) {
            return axios.post(url, data).then((response) => {
                if (response.status == 200) {
                    return response.data;
                }
            });
        },
        save: function () {

        }
    },
    watch: {
        selectedShopId: function(value) {
            this.get_categories();
        }
    }
});

