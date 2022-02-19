Vue.component('itemsaver', {
    template: `
        <div class="row col-sm-12 z-5 bg-light">
            <table class="table word-break">
                <tbody>
                    <tr>
                        <th>
                            <span>Image: </span>
                        </th>
                        <td>
                            <img :src="sourceItem.image">
                        </td>
                        <td>
                            <img :src="destinationItem.image">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Image url: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.image }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.image">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Label: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.label }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.label">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Category: </span>
                        </th>
                        <td>
                            <span>{{ sourceLabels.categoryLabel }} ({{ sourceItem.category }})</span>
                        </td>
                        <td class="input-group">
                            <select class="form-select" v-model="destinationItem.category">
                                <option disabled>Chose category</option>
                                <option v-for="category in categories" v-bind:value="category.id">{{ category.label }}</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Brand: </span>
                        </th>
                        <td>
                            <span>{{ sourceLabels.brandLabel }} ({{ sourceItem.brand }})</span>
                        </td>
                        <td class="input-group">
                            <select class="form-select" v-model="destinationItem.brand">
                                <option disabled>Chose brand</option>
                                <option v-for="brand in brands" v-bind:value="brand.id">{{ brand.label }}</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Consist: </span>
                        </th>
                        <td>
                            <span v-for="consistLabel in sourceLabels.consistLabels">{{ consistLabel }}</span>
                        </td>
                        <td class="input-group">
                            <select class="form-select" multiple v-model="destinationItem.consist">
                                <option disabled>Chose consists</option>
                                <option v-for="consist in consists" v-bind:value="consist.id">{{ consist.label }}</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Units: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.units }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.units">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Term: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.term }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.term">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Calorie: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.calorie }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.calorie">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Carbohydrates: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.carbohydrates }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.carbohydrates">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Fat: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.fat }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.fat">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Proteins: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.proteins }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.proteins">
                        </td>
                    </tr>
                </tbody>
            </table>
            <button class="btn btn-primary" v-on:click='$emit("insert")'></button>
        </div>
    `,
    props: {
        sourceItem: {
            type: Object
        },
        destinationItem: {
            type: Object
        }
    },
    computed: {
        sourceLabels: function(){
            var itemCategoryLabels = this.$labels.categories.filter(value => value.id == this.sourceItem.category);
            var itemBrandLabels = this.$labels.brands.filter(value => value.id == this.sourceItem.brand);
            var itemPackageLabels = this.$labels.packages.filter(value => value.id == this.sourceItem.package);
            var itemConsistLabels = this.sourceItem.consist !== null && this.sourceItem.consist.length > 0 ? this.$labels.consists.filter(value => this.sourceItem.consist.includes(value.id)) : [];
            var itemCountryLabels = this.$labels.countries.filter(value => value.id == this.sourceItem.additional.country);
    
            return {
                categoryLabel: itemCategoryLabels.length > 0 ? itemCategoryLabels[0].label : null,
                brandLabel: itemBrandLabels.length > 0 ? itemBrandLabels[0].label : null,
                packageLabel: itemPackageLabels.length > 0 ? itemPackageLabels[0].label : null,
                consistLabels: itemConsistLabels.length > 0 ? itemConsistLabels.map(value => value.label) : [],
                countryLabel: itemCountryLabels.length > 0 ? itemCountryLabels[0].label : null
            };
        },
        categories: function(){
            return this.$labels.categories;
        },
        brands: function(){
            return this.$labels.brands;
        },
        consists: function(){
            return this.$labels.consists;
        }
    }
});

